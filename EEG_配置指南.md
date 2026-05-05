# EEG 情绪系统配置指南

## 📋 快速配置步骤

### 1. 获取 websocket-sharp.dll

由于GitHub上没有预编译版本，使用以下方案之一：

#### 方案A：通过Unity Package Manager（推荐）
1. 打开Unity → Window → Package Manager
2. 点击 "+" → "Add package from git URL..."
3. 输入：`https://github.com/sta/websocket-sharp.git`
4. 等待安装完成

#### 方案B：手动放置dll文件
1. 找到或编译 `websocket-sharp.dll`
2. 复制到：`My project/Assets/Plugins/`

---

### 2. 在Unity中设置

#### 2.1 创建GameObject并挂载脚本
1. 打开Unity，加载 `SampleScene`
2. Hierarchy → 右键 → Create Empty
3. 重命名为 "EEG_Emotion_Receiver"
4. Project窗口 → 找到 `Scripts/EmotionReceiver.cs`
5. 拖拽到 "EEG_Emotion_Receiver" GameObject上

#### 2.2 配置Inspector参数

##### WebSocket Settings
- Server URL: `ws://localhost:8765`
- Reconnect Delay: `3`

##### Skybox Settings
从 `Assets/SpaceSkies Free/` 中选择：
- Happy Skybox: `Skybox_1/Pink_4K_Resolution.mat`（温暖/明亮）
- Sad Skybox: `Skybox_2/Green_4K_Resolution.mat`（蓝色/冷色）
- Normal Skybox: `Skybox_3/Purple_4K_Resolution.mat`（中性色）

##### Music Settings
从 `Assets/Audio/` 中选择：
- Happy Music: `Cry for the moon.mp3`（或其他快乐音乐）
- Sad Music: `Look up to the Stars.mp3`（或其他悲伤音乐）
- Normal Music: `Weightless.mp3`（或其他平静音乐）
- Music Transition Time: `2`

##### Debug
- Show Debug Info: ✅ **勾选**（这样就能看到调试界面了！）

---

### 3. 启动Python后端

打开命令行，执行：

```bash
cd d:\AR\AR\realtime_inference
pip install -r requirements.txt
python main.py -c config/config.yaml
```

---

### 4. 运行Unity场景

1. 在Unity中点击Play按钮
2. 您会看到左上角的调试界面：
   - 📊 EEG Emotion Receiver
   - 🔗 Connection: ✅ Connected
   - 😊 Current Emotion: NORMAL
   - 🎯 Confidence: 0.79
   - ⏱️ Transition: 0.00
   - 🟢 ● LIVE

---

## 🎨 可选：使用 EmotionEffectConfig（高级功能）

如果您想使用更高级的情感效果系统：

1. **创建配置文件**
   - Project窗口 → 右键 → Create → Scriptable Objects → Emotion Effect Config
   - 命名为 "MyEmotionConfig"

2. **配置情感效果**
   - 分别配置 Happy/Sad/Normal 三种状态：
     - Skybox: 天空盒材质
     - Ambient Color: 环境光颜色
     - Light Color: 光源颜色
     - Light Intensity: 光源强度
     - Volume Profile: 后处理配置
     - Background Music: 背景音乐
     - Transition Sound: 过渡音效
     - Ambient Particles: 环境粒子
     - Transition Particles: 过渡粒子
     - Particle Duration: 粒子持续时间
     - Screen Flash Color: 屏幕闪烁颜色
     - Flash Duration: 闪烁持续时间
     - Transition Duration: 过渡持续时间

3. **使用 EmotionEffectManager**
   - 同第2步，创建GameObject并挂载 `EmotionEffectManager.cs`
   - 在Inspector中分配 `MyEmotionConfig`
   - 配置 Camera、Light、Particle Spawn Point、Volume等引用

---

## 🛠️ 故障排除

### 问题1：找不到 websocket-sharp.dll
- **解决方案**：确保 dll 文件在 `Assets/Plugins/` 目录下
- **检查**：在Unity中 Project 窗口查看是否能看到 dll

### 问题2：Unity无法连接到Python后端
- **检查**：Python服务是否正在运行
- **确认**：端口8765是否被占用
- **检查防火墙设置**

### 问题3：编译错误
- **确保**：Unity版本兼容（建议2020.3或更高）
- **检查**：所有脚本都在 Assets/Scripts/ 目录下

---

## 📞 需要帮助？

如果您在配置过程中遇到任何问题，请参考：
- `README_realtime_inference.md` - 详细的后端说明
- `realtime_inference/config/config.yaml` - 后端配置文件
- `realtime_inference/THINKGEAR_SETUP_GUIDE.md` - ThinkGear设备设置指南

---

## ✅ 配置检查清单

- [ ] websocket-sharp.dll 在 Assets/Plugins/ 目录下
- [ ] EmotionReceiver.cs 已挂载到 GameObject
- [ ] Server URL 已设置为 ws://localhost:8765
- [ ] 三个Skybox Materials 都已分配
- [ ] 三个Music Clips 都已分配
- [ ] Show Debug Info 已勾选
- [ ] Python后端服务已启动
- [ ] Unity场景正在运行
- [ ] 可以看到调试界面！

---

最后更新：2026-04-24
