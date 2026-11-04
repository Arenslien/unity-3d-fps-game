# AI와 함께하는 그래픽 품질 개선 및 최적화 보고서

- 프로젝트: Unity 6 (6000.3.22f1) URP 좀비 FPS (`GameScene`)
- 작성일: 2026-11-02
- 측정 환경: Windows 11, NVIDIA GeForce RTX 4060 Laptop GPU, Direct3D12, Unity 에디터 Play 모드, Game 뷰 1920x1080
- AI 활용: Claude Code(Unity MCP 연동)로 씬 상태 조회, 조명 배치, Volume 생성, 값 튜닝, 스크린샷/수치 수집, 보고서 초안 작성

> 이미지 첨부 방법: 아래 `📷 파일명`은 `Docs/report/` 폴더의 파일입니다. 노션에 붙여 넣은 뒤 해당 위치에 이미지를 드래그해서 넣으면 됩니다.

---

## 1. 목표와 콘셉트

**콘셉트: "달빛 아래 버려진 폐공장에서 좀비와 싸우는 밤"**

- 낮 맵을 밤으로 바꾸고, 가로등과 모닥불로 길목만 밝혀 플레이어가 빛을 따라 이동하게 만든다.
- 포스트 프로세싱으로 채도를 낮춘 거친 화면을 만들고, 특정 구역은 흑백 영화처럼 분위기를 완전히 바꾼다.
- 모든 변경은 같은 위치/같은 시점에서 Before/After를 비교한다.

---

## 2. 조명 (Lighting)

### 2-1. 적용 내용

| 항목 | Before | After |
|---|---|---|
| 태양(Directional Light) | 낮 조명, Intensity 1.5 | 달빛: 색 (0.45, 0.55, 0.9), Intensity 0.35 |
| Skybox | 기본 노출 | Exposure 0.12, Tint 푸른색 |
| 환경광(Ambient) | 기본 | Flat (0.06, 0.08, 0.14) 어두운 남색 |
| Fog | 없음 | Exponential Squared, Density 0.012 |
| 가로등 | 없음 | 22개, Spot Light, **Mixed**, 따뜻한 색 (1, 0.82, 0.55), Intensity 60, Range 16 |
| 모닥불 | 없음 | 12개, VFX + Point Light, **Mixed**, 주황 (1, 0.5, 0.15), Intensity 25, Range 9 |
| Light Probe | 없음 | 모닥불 12곳 주변에 Light Probe Group, 총 369개 |
| Lightmap | 낮 조명 기준 | 밤 조명 기준 재베이크 (Shadowmask) |

- 가로등/모닥불은 NavMesh 기준으로 **플레이어가 실제 걸어갈 수 있는 길목**에 배치했다.
- 가로등 에셋 머티리얼이 Built-in `Standard` 셰이더라 URP에서 분홍색으로 보여서, **URP/Lit으로 변환**했다. 전구 머티리얼(`Light.mat`)은 Emission 1024 → (3, 2.4, 1.5)로 낮춰 Bloom에 적당히 걸리게 했다.

### 2-2. Realtime / Mixed / Baked 비교 (학습 내용)

| 모드 | 직접광 | 간접광(반사광) | 그림자 | 비용 |
|---|---|---|---|---|
| Realtime | 실시간 | 없음(우리 프로젝트는 Realtime GI 꺼짐) | 실시간 | 높음 |
| Mixed (Shadowmask) | 실시간 | **Lightmap/Light Probe에 베이크** | 정적 그림자 베이크 + 동적 실시간 | 중간 |
| Baked | 베이크 | 베이크 | 베이크 | 낮음 (움직이는 물체에 직접광 없음) |

- 모닥불을 Realtime → **Mixed**로 바꾼 이유: Realtime은 간접광이 없어서 불 주변 바닥과 벽에 번지는 빛이 없었다. Mixed로 바꾸고 베이크하자 불빛이 주변에 번진다.
- 콘솔의 `Realtime indirect bounce shadowing is only supported for Directional lights` 메시지는 "Directional이 아닌 실시간 조명은 간접광 그림자를 지원하지 않는다"는 안내이며, 오류가 아니다.

### 2-3. Light Probe

- Light Probe는 베이크된 간접광을 공간의 점에 저장했다가, **움직이는 오브젝트(좀비, 플레이어)**에 입혀 준다.
- 프로브가 하나면 공간 안에서 밝기 변화를 표현하지 못한다. 그래서 모닥불마다 **2개 높이(0.5m, 1.5m) × 중심 + 반지름 1.8m 링 8개 + 반지름 4m 링 8개**로 감싸서, 불에 가까워질수록 밝아지게 했다.
- 콜라이더 안에 들어간 프로브는 검게 베이크되므로 제외했다.
- Adaptive Probe Volume(APV)도 검토했으나, Light Probe Group과 같이 쓸 수 없고 URP 설정 변경이 필요해서 **Light Probe Group만 사용**했다.

### 2-4. Lightmap 베이크 최적화 (Before/After 수치)

| 항목 | Before | After |
|---|---|---|
| Lightmap Resolution | 30 (texels/unit) | **20** |
| 베이크 시간 | 약 5분 | **86초** |
| Lightmap 개수 | 50장 | **12장** (1024², BC6H) |
| 예상 런타임 메모리 | 약 403 MB | **약 96 MB** (약 76% 감소) |
| 베이크된 Light Probe | 0개 | 369개 |

- 분석: 베이크 시간은 조명 개수가 아니라 **해상도 × 샘플 수 × 씬 크기**가 좌우한다. 밤 장면은 어두워서 해상도를 낮춰도 품질 차이가 눈에 잘 띄지 않아, 해상도를 낮추는 것이 이득이 컸다.
- 주의: 처음 확인한 이전 해상도 값이 40으로 읽혔다가 30으로 다시 확인되어, 이전 베이크의 정확한 해상도는 30~40 사이로 불확실하다.

### 2-5. 그림자

- 가로등은 22개라 모두 실시간 그림자를 켜면 그림자 맵 렌더링 비용이 크게 늘어난다. 그래서 기본은 **No Shadows**로 두고, 한 개만 Soft Shadows로 켜서 차이를 비교했다.
- URP 설정: 그림자 거리 50, 메인/추가 조명 그림자 해상도 2048, Cascade 4.
- 측정 시점 Shadow Casters: **595** (아래 성능 표 참고). 최적화 후보로 남겨 둔다.

📷 (조명 Before/After 이미지가 따로 있으면 여기에 첨부)

---

## 3. 포스트 프로세싱

### 3-1. 구성

| 오브젝트 | 종류 | Profile | Priority | 설명 |
|---|---|---|---|---|
| `Global Volume` | Global | `SampleSceneProfile` (템플릿) | 0 | **Before** 비교용으로 남겨 두고 비활성화 |
| `MapVolume` | Global | `Settings/Volume/ZombieMapProfile` | 0 | 맵 전체 좀비 FPS 분위기 |
| `Warehouse_NoirVolume` | Local (Box, Is Trigger) | `Settings/Volume/ZombieNestNoirProfile` | **10** | 창고 앞 모닥불 마당(중심 -21, 3, 14 / 크기 16×8×14)에 들어가면 흑백 영화 |

- Main Camera: Post Processing 켬, Anti-aliasing **None → SMAA**
- MinimapCamera: Post Processing 꺼짐 유지 (미니맵에는 효과를 적용하지 않음)

### 3-2. MapVolume (ZombieMapProfile) 최종값

| 효과 | 값 | 의도 |
|---|---|---|
| Tonemapping | Neutral | HDR 밝은 빛을 자연스럽게 압축 |
| Bloom | Threshold 1, Intensity 0.8, Scatter 0.7, Tint (1, 0.85, 0.7), HQ Filtering 끔 | 가로등과 불빛이 따뜻하게 번짐 |
| Color Adjustments | Post Exposure 0.4, Contrast 15, Saturation -15, Color Filter (0.85, 0.95, 0.92) | 채도를 낮추고 약간 병든 청록 톤 |
| Split Toning | 어두운 부분 청록, 밝은 부분 주황, Balance -20 | 어둠은 차갑게, 불빛은 따뜻하게 |
| Vignette | Intensity 0.3, Smoothness 0.4, 검정 | 시선을 화면 중앙(조준점)으로 모음 |
| Film Grain | Medium1, Intensity 0.2 | 거친 공포 영화 질감 |
| Chromatic Aberration | Intensity 0 | 피격 효과(교안 24.10.2)용 자리, 평소엔 0 |
| Motion Blur / Depth of Field | 끔 | FPS에서 조준 방해, 교안 규칙 |

### 3-3. 흑백 영화 구역 (ZombieNestNoirProfile)

| 효과 | 값 |
|---|---|
| Color Adjustments | **Saturation -100**, Contrast 45, Post Exposure -1.2 |
| Split Toning | 회색/회색 (색 틴트 제거) |
| Vignette | Intensity 0.6, Smoothness 0.55 |
| Film Grain | Thin2, Intensity 0.7 (오래된 필름 느낌) |
| Bloom | Intensity 0.3 |

- Priority 10으로 맵 Volume보다 우선 적용된다.
- Blend Distance 2m: 구역 경계 2m 바깥부터 서서히 흑백으로 바뀐다.
- 처음에는 좀비 스포너가 모인 "좀비 둥지"에 배치했으나, 그곳은 **조명이 하나도 없어 흑백으로 바꿔도 검은 화면이 그대로 검은 화면**이었다. 효과가 보이도록 조명이 있는 모닥불 마당으로 옮겼다.

### 3-4. 결과 스크린샷 (같은 위치/같은 시점)

**지점 A: 가로등과 모닥불이 보이는 길목**

| Before (템플릿 Volume, AA 없음) | After (MapVolume, SMAA) |
|---|---|
| 📷 `01_A_before.png` | 📷 `05_A_after_final.png` |

**지점 B: 창고 앞 모닥불**

| Before | After (MapVolume) | 흑백 구역 안 | 흑백 구역 경계(Blend) |
|---|---|---|---|
| 📷 `06_B_before.png` | 📷 `07_B_after_final.png` | 📷 `08_B_noir_inside.png` | 📷 `09_B_noir_blend.png` |

- After는 채도가 낮아지고, 불빛 주변만 따뜻하게 남고, 화면 가장자리가 어두워져 시선이 중앙에 모인다.
- 흑백 구역에 들어가면 색이 완전히 빠지고 대비와 필름 노이즈가 강해져 분위기가 정반대로 바뀐다.
- 경계 구간에서는 지붕에 약간 따뜻한 색이 남아 있어 Blend Distance로 반쯤 섞이는 것을 확인할 수 있다.

### 3-5. AI와 함께한 튜닝 과정 (실패 포함)

| 시도 | 변경 | 결과 | 📷 |
|---|---|---|---|
| 1차 | ACES, Exposure 0.3, Contrast 18, Saturation -25, SSAO 1 | **실패**: 화면 대부분이 검게 뭉개짐 | `02_A_after_try1_ACES.png` |
| 2차 | Exposure 1.0, Contrast 10 | 조금 나아졌지만 Before보다 어두움 | `03_A_after_try2_exposure.png` |
| 3차 | 교안 기본값 Neutral, Exposure 0.4, Contrast 15, Saturation -15 | 여전히 어두움 → 톤매핑 문제가 아님 | `04_A_after_try3_SSAO1.png` |
| 4차 | **SSAO Intensity 1 → 0.4** | 해결. 밝기는 Before 수준, 분위기만 바뀜 | `05_A_after_final.png` |

**실패 분석**

1. **ACES 톤매핑 + 높은 Contrast**: ACES는 어두운 영역을 더 눌러 주는 곡선이라, 원래 어두운 밤 장면에서는 그림자 부분이 다 검게 뭉개졌다.
2. **SSAO Intensity 1 (교안 권장값)**: 교안 예시는 밝은 공장 장면 기준이다. 우리 밤 장면은 환경광이 거의 없어서, AO를 세게 걸면 남아 있는 간접광까지 지워져 화면 전체가 꺼졌다. 한 번에 하나씩 되돌려 보면서 원인을 SSAO로 좁혔다. → **밤 장면은 SSAO 0.4 유지가 최적**이라고 판단.
3. 결론: 권장값은 장면의 밝기 조건에 따라 달라진다. 효과를 하나씩 켜고 끄며 비교해야 원인을 찾을 수 있다.

### 3-6. 안티에일리어싱

- None → **SMAA**로 변경. 철조망과 건물 모서리의 계단 현상이 줄어든다.
- MSAA는 URP Asset에서 꺼져 있고(1x), TAA는 움직일 때 잔상이 생겨 FPS에 맞지 않아 SMAA를 선택했다.

---

## 4. 성능: 최적화 이전 기준값 (Baseline)

측정 조건: 지점 A, 최종 그래픽 설정(MapVolume + SMAA + SSAO 0.4), Game 뷰 1920x1080, 좀비 AI 정지(촬영 중 공격 방지), Game 창 포커스.

📷 `stats_before.png` (Game 뷰 + Stats)
📷 `stats_before_editor.png` (에디터 전체)
📷 `profiler_cpu_before.png` / `profiler_gpu_before.png` / `profiler_memory_before.png` / `profiler_rendering_before.png`

### 4-1. CPU / GPU

| 항목 | 값 | 출처 |
|---|---|---|
| FPS | **190.5 FPS (5.3 ms)** | Stats |
| CPU main | 5.3 ms | Stats |
| Render thread | 4.2 ms | Stats |
| CPU 프레임 (Profiler) | 5.72 ms | Profiler GPU 모듈 |
| GPU 프레임 | **3.72 ms** | Profiler GPU 모듈 |
| PlayerLoop | 2.47 ms | Profiler CPU Timeline |

### 4-2. 렌더링

| 항목 | 값 |
|---|---|
| Batches | **776** (Saved by batching 327) |
| SetPass Calls | **125** |
| Draw Calls | 983 |
| Triangles / Vertices | **4.5M / 3.5M** |
| Shadow Casters | **595** |
| Visible Skinned Meshes | 99 |
| Render Textures | 65개 / 221.5 MB |
| Used Textures | 122개 / 115.5 MB |

### 4-3. 메모리 (RAM)

| 항목 | 값 |
|---|---|
| Textures | **1338개 / 0.70 GB** |
| Meshes | 594개 / 97.4 MB |
| Materials | 257개 / 0.7 MB |
| Audio | 2.3 MB |
| Graphics & Graphics Driver | 0.96 GB |
| Managed Heap (사용/예약) | 0.89 / 1.08 GB |
| GC allocated in frame | 0 B |

### 4-4. 측정 시 주의점

- **에디터 측정값이다.** Memory 모듈에 "에디터 메모리는 빌드와 다르다"는 경고가 있으며, Profiler 자체가 4 GB를 쓰고 있다. 빌드 측정보다 수치가 크게 나온다.
- Game 창에 포커스가 없거나 일시정지 상태에서는 FPS가 크게 떨어진다(측정 중 5.7~15.9 FPS로 표시된 순간이 있었음). 위 표는 Game 창에 포커스를 두고 실행 중인 프레임 기준이다.
- 같은 조건 비교를 위해 After 측정도 **같은 위치(지점 A), 같은 해상도, Game 창 포커스** 상태에서 해야 한다.

### 4-5. Baseline 분석: 병목 후보

| 영역 | 관찰 | 기대 효과가 큰 최적화 후보 |
|---|---|---|
| GPU | Tris 4.5M, Shadow Casters 595 | 그림자 거리 50 → 30, 작은 소품의 Cast Shadows 끄기, 오클루전 컬링 베이크(이전 테스트 씬에서 학습) |
| GPU | Batches 776, SetPass 125 | 정적 오브젝트 Static Batching 확인, 머티리얼 수 줄이기 |
| GPU | Visible Skinned Meshes 99 | 좀비 모델 LOD 또는 컬링 |
| RAM | Textures 0.70 GB (1338개) | 큰 텍스처 Max Size 4096 → 2048/1024, 압축 형식 확인 |
| RAM | Lightmap | (이미 적용) 403 MB → 96 MB |
| CPU | PlayerLoop 2.47 ms, GC 0 B | 현재 CPU 여유 있음. 히트 이펙트 `Instantiate` → 풀링은 코드 변경이 필요해 선택 사항 |

---

## 5. 최적화 적용 결과

| # | 최적화 | Before | After | 변화 |
|---|---|---|---|---|
| 1 | Lightmap 해상도 30 → 20 | 50장 / 약 403 MB / 약 5분 | 12장 / 약 96 MB / 86초 | 메모리 약 76% 감소, 베이크 시간 약 71% 단축 |
| 2 | (선택한 GPU 최적화) | Shadow Casters 595, Batches 776 | | |
| 3 | (선택한 RAM 최적화) | Textures 0.70 GB | | |

> 2, 3번은 Plan 모드로 후보를 정해 적용한 뒤, 4장과 같은 조건으로 다시 측정해서 채운다.

---

## 6. 정리

- 조명: 낮 → 밤 전환, Mixed 조명과 Light Probe로 움직이는 좀비에도 불빛이 입혀지게 했고, Lightmap 해상도 조정으로 메모리를 약 76% 줄였다.
- 포스트 프로세싱: 맵 전체에 좀비 FPS 분위기(저채도, 따뜻한 불빛, 비네트, 필름 노이즈)를 입히고, Local Box Volume으로 특정 구역을 흑백 영화처럼 바꿨다.
- 배운 점: 교안 권장값(SSAO 1, ACES)이 밤 장면에서는 오히려 화면을 망쳤다. **효과를 하나씩 켜고 끄며 원인을 분리하는 과정**이 AI와 함께 최적값을 찾는 데 가장 중요했다.
- 성능 기준값(190 FPS, Batches 776, SetPass 125, Tris 4.5M, Textures 0.70 GB)을 확보했으며, 이를 기준으로 GPU/RAM 최적화를 이어서 진행한다.
