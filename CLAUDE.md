# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

Unity **6000.3.22f1**(Unity 6) + URP 기반 FPS 학습 프로젝트. 씬은 `Assets/01.Scenes/GameScene.unity` 하나뿐이다.
게임 코드는 전부 `Assets/02.Scripts/` 아래에 있으며 asmdef 없이 기본 `Assembly-CSharp`로 컴파일된다. `Assets/Modules/Submodules/Pools`, `Assets/Plugins`(Lean Pool 등), `BloodDecalsAndEffects`, `TextMesh Pro`는 외부 에셋이므로 수정 대상이 아니다.

## 명령어

빌드·실행·플레이 테스트는 Unity 에디터에서 한다(CLI 빌드 스크립트 없음). 테스트 코드는 없다 — `Assets/02.Scripts/Test/`는 C# 문법 학습용 예제이지 단위 테스트가 아니다.

```bash
# 최초 1회: husky 훅 설치
dotnet tool restore
dotnet husky install

# 코드 스타일 검사 (pre-commit 훅과 동일)
dotnet format unity-3d-fps-game.sln --verify-no-changes --severity warn --include Assets/02.Scripts/

# 자동 수정
dotnet format unity-3d-fps-game.sln --severity warn --include Assets/02.Scripts/
```

`.sln`/`.csproj`는 Unity가 생성하므로, 파일을 추가한 뒤 `dotnet format`이 새 파일을 못 찾으면 에디터에서 프로젝트를 다시 열어 재생성해야 한다.

## Git 규칙

- **커밋 메시지**: `.husky/commit-msg`가 `type: 설명` 형식을 강제한다. 허용 type: `asset|build|chore|ci|docs|feat|fix|perf|refactor|revert|style|test` (선택적 `(scope)`, `!`). 설명은 한국어로 작성한다 (예: `feat: 미니맵 확대 축소 기능 추가`).
- **pre-commit**: 위 `dotnet format --verify-no-changes`가 실패하면 커밋이 막힌다.
- **CI**: `main`에 `Assets/02.Scripts/**/*.cs` 변경이 push되면 `.github/workflows/gemini-commit-review.yml`이 Gemini로 커밋을 리뷰해 커밋 코멘트를 남긴다. 리뷰 기준은 `Docs/CodeReviewGuide.md`.

## 코드 스타일 (`.editorconfig`, `Assets/02.Scripts/**.cs`에만 적용)

- 4칸 스페이스, 중괄호는 항상 새 줄(Allman).
- 한 줄 조기 리턴은 중괄호 생략 허용(`if (...) return;`), 여러 줄이면 중괄호 필수.
- private 필드 `_camelCase`, private `const` / `static readonly`는 `PascalCase`, 지역 변수 `camelCase`, 타입명은 `PascalCase`.
  - 단, UI 클래스는 `UI_` 접두사 관례(`UI_Minimap`, `UI_HitEffect`)를 쓰고 있다.
- 코루틴 메서드는 `Xxx_Coroutine` 이름을 쓴다.
- Inspector 노출 값은 `[SerializeField] private`로, `[Header("스탯")]` 등으로 그룹화한다. 주석은 한국어.

## 아키텍처

### 게임 상태 게이트
`GameManager`(싱글톤, `GameManager.Instance`)가 `GameState { Ready, Playing, GameOver }`를 가진다. 시작 시 "준비!" → "시작!" 코루틴 후 `Playing`으로 전환된다.
**입력·AI를 처리하는 모든 `Update()`는 첫 줄에서 `GameManager.Instance.State != GameState.Playing`이면 리턴한다.** 새 게임플레이 컴포넌트도 이 패턴을 따라야 한다.

### 커서 상태 = 입력 모드
`CursorManager`가 Left Alt를 누르는 동안 커서를 해제한다. 별도 플래그 없이 **`Cursor.visible`을 직접 읽어** "UI 조작 중" 여부를 판단하며, 이때 사격(`PlayerGunFire`)과 플레이어/카메라 회전(`PlayerRotate`, `CameraRotate`)이 멈춘다.

### 대미지 시스템
`IDamageable.TakeDamage(DamageInfo)` 인터페이스 하나로 통일되어 있다. `DamageInfo`는 대미지량과 피격 위치/법선을 담는 struct.
- 공격하는 쪽은 구체 타입을 모른 채 `collider.GetComponent<IDamageable>()?.TakeDamage(...)`로 호출한다 (`PlayerGunFire`의 레이캐스트, `Bomb`/`Drum`의 `Physics.OverlapSphere` 범위 대미지).
- 구현체: `Player`, `Zombie`, `Drum`, `Box`. 범위 대미지는 `"Drum"`, `"Zombie"` **레이어**로 대상을 거르므로, 새 피격 대상을 범위 공격에 포함하려면 레이어 설정과 `LayerMask.GetMask(...)` 호출을 함께 수정해야 한다.
- 드럼통 폭발은 주변 드럼통에 연쇄 대미지를 준다.

### 플레이어
역할별 컴포넌트로 분리되어 있다: `Player`(체력·스태미나 데이터, 사망 시 `GameManager.GameOver()`), `PlayerMove`, `PlayerRotate`, `PlayerGunFire`(히트스캔, 탄창/재장전 코루틴), `PlayerBombFire`(우클릭 충전 투척). 이동·사격은 **레거시 `Input` API**(`Input.GetButton("Fire1")`, `"Reload"` 등 Input Manager 축)를 쓴다 — Input System 패키지도 설치되어 있지만(Active Input Handling = Both) 코드에서는 사용하지 않는다.

### 오브젝트 풀
수류탄은 `PlayerBombPool`(싱글톤, `Queue<Bomb>`, 고정 크기 10)에서 꺼내 쓰고 `Bomb`이 충돌 시 `ReturnBomb`으로 반납한다. 반면 히트 이펙트·폭발 이펙트·피 이펙트는 아직 `Instantiate`로 생성한다.

### 좀비 AI
`Zombie`는 `ZombieState` enum + `switch` 기반 FSM(Idle/Trace/Comeback/Attack/Hit/Death). `Hit`·`Death`는 `Update`에서 처리하지 않고 코루틴으로 상태를 되돌리거나 파괴한다. `CharacterController.SimpleMove`로 이동하며 NavMesh는 사용하지 않는다. 플레이어는 `"Player"` 태그로 찾는다.

### UI
`UI_PlayerStats`처럼 UI는 `LateUpdate`에서 모델(`Player`)의 프로퍼티를 폴링해 갱신한다(이벤트 기반 아님). 미니맵은 별도 `MinimapCamera` + 마스크로 원형 표시하고, 버튼 `OnClick`으로 확대/축소한다.

### 로깅
`GameLogger.Log`는 `[Conditional("ENABLE_LOG")]`이므로 심볼이 없으면 호출 자체가 컴파일에서 제거된다(Standalone에 `ENABLE_LOG` 정의됨). 기존 코드에는 `Debug.Log` 직접 호출도 섞여 있다.

## 코드 리뷰

리뷰 요청 시 `Docs/CodeReviewGuide.md`의 기준(중요도 Critical/High/Medium/Low, 총평 → 발견된 문제 → 설계 개선 제안 → 잘 작성된 점 형식, 한국어)을 따른다. 학습 프로젝트이므로 과도한 추상화나 패턴 강요는 피한다.
