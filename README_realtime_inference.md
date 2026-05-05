# EEG Emotion Real-time Inference System (脑电情感实时推理系统)

## 📋 项目概述

本目录包含 **EEG Emotion 项目的实时推理系统**，用于从脑电图(EEG)数据中实时推断用户情感状态，并将结果通过 WebSocket 发送到 Unity AR/VR 应用。

该系统是 **EEG+AI+VR** 技术栈的核心组件之一，实现了从脑电信号采集、特征提取、模型推理到可视化呈现的完整实时处理流程。

---

## 🔗 上游项目资源

⚠️ **重要提示**: 本项目为 EEG Emotion 模块化工程的**实时推理子系统**。

如需获取完整的训练框架、模型训练代码、数据处理工具等完整资源，请访问：

🌐 **[https://github.com/ThirteenAsh/eeg_modular](https://github.com/ThirteenAsh/eeg_modular)**

### eeg_modular 项目提供：

- ✅ 统一的数据预处理接口
- ✅ 多种模型支持（SVM、MLP、RF、XGBoost、LSTM、CNN、混合模型）
- ✅ 配置驱动的训练流程
- ✅ 丰富的可视化输出（混淆矩阵、UMAP边界图、训练曲线）
- ✅ 多模型比较与评估工具
- ✅ 混合模型集成（soft voting + stacking）

---

## 🏗️ 系统架构

```
realtime_inference/
├── main.py                    # 主程序入口
├── config/
│   └── config.yaml            # 系统配置文件
├── src/
│   ├── model.py               # 模型推理模块（多模态CNN/CVAE）
│   ├── voting.py              # 滑动窗口投票机制
│   ├── unity_comm.py          # Unity WebSocket通信
│   └── thinkgear.py           # ThinkGear设备数据采集
├── models/
│   └── best_fold4.pt          # 预训练模型权重
├── features/                  # 特征标准化器（scaler, encoder）
│   ├── scaler_att.joblib
│   ├── scaler_med.joblib
│   ├── scaler_powerspec.joblib
│   ├── scaler_filtered.joblib
│   ├── label_encoder.joblib
│   └── onehot_encoder.joblib
├── unity/
│   └── EmotionReceiver.cs     # ⭐ Unity端接收脚本（关键桥梁组件）
├── requirements.txt           # Python依赖
└── THINKGEAR_SETUP_GUIDE.md   # ThinkGear硬件设置指南
```

---

## 🎯 核心功能

### 1. 实时数据采集
- 支持 **NeuroSky ThinkGear** 脑电传感器
- TCP/串口双模式连接
- 多模态特征提取：filtered、powerspec、attention、meditation

### 2. 情感推理引擎
- **多模态 CNN + CVAE** 架构
- 支持三种情感分类：**happy（高兴）、sad（悲伤）、normal（平静）**
- 实时概率输出与置信度评估

### 3. 平滑过渡机制
- **滑动窗口投票器**：避免情感状态频繁跳变
- **平滑过渡动画**：支持渐变式情感切换
- 可配置的稳定帧数和阈值参数

### 4. Unity集成通信
- **WebSocket** 双向通信（端口 8765）
- JSON格式数据传输
- 自动重连机制
- 心跳检测保活

---

## 🌉 关键桥梁组件：EmotionReceiver.cs

### ⭐ 重要说明

**文件位置**: `unity/EmotionReceiver.cs`  
**状态**: ✅ **必须保留并使用的关键文件**

#### 功能描述

[EmotionReceiver.cs](unity/EmotionReceiver.cs) 是连接 **Python 推理后端** 与 **Unity AR 前端** 的核心桥梁脚本，负责：

1. **WebSocket 连接管理**
   - 自动连接到 Python 推理服务器（默认 `ws://localhost:8765`）
   - 断线自动重连（3秒间隔）
   - 心跳保活机制

2. **实时情感数据接收**
   - 解析 JSON 格式的情感数据包
   - 提取情感标签、置信度、过渡进度、概率分布

3. **场景动态响应**
   - **天空盒过渡**：根据情感状态平滑切换天空盒材质
     - happy → 明亮温暖的天空盒
     - sad → 阴沉冷静的天空盒
     - normal → 默认中性天空盒
   - **音乐淡入淡出**：交叉淡化背景音乐
   - **可配置的过渡时间**

4. **调试界面**
   - OnGUI 显示连接状态、当前情感、置信度等信息

#### 数据协议

```json
{
  "emotion": "happy",
  "confidence": 0.85,
  "transition_progress": 0.7,
  "probabilities": {
    "happy": 0.85,
    "sad": 0.10,
    "normal": 0.05
  },
  "timestamp": 1686739200.123
}
```

#### 使用方法

1. 在 Unity 中创建空 GameObject
2. 挂载 `EmotionReceiver.cs` 脚本
3. 在 Inspector 中配置：
   - **Server URL**: WebSocket 服务器地址（默认 `ws://localhost:8765`）
   - **Skybox Materials**: 三种情感对应的天空盒材质
   - **Music Clips**: 三种情感对应的音频片段
   - **Transition Times**: 过渡动画时长
4. 运行 Unity 场景，脚本会自动连接并接收情感数据

#### 完整性保证

✅ **确保此文件的完整性**:
- 该文件位于 `realtime_inference/unity/` 目录下
- 是本项目与 [eeg_modular](https://github.com/ThirteenAsh/eeg_modular) 生态系统的唯一连接点
- 修改或删除此文件将导致 Unity 端无法接收脑电情感数据
- 已在 `.gitignore` 中排除规则外，确保被版本控制追踪

---

## 🚀 快速开始

### 前置条件

- Python 3.8+
- PyTorch 2.0+
- NeuroSky ThinkGear 设备（或使用 Mock 模式）
- Unity 2022+ （用于运行 EmotionReceiver.cs）

### 安装依赖

```bash
cd realtime_inference
pip install -r requirements.txt
```

### 配置系统

编辑 `config/config.yaml`:

```yaml
thinkgear:
  connection_mode: "tcp"      # 或 "serial"
  com_port: "COM6"           # 串口模式下的端口号
  use_mock: false             # 设为 true 可使用模拟数据测试

model:
  path: "models/best_fold4.pt"
  device: "auto"             # auto, cpu, cuda

unity:
  host: "localhost"
  port: 8765                 # WebSocket 端口
```

### 启动推理服务

```bash
python main.py -c config/config.yaml
```

### 启动 Unity 客户端

1. 打开 Unity 项目
2. 确保 `EmotionReceiver.cs` 已挂载到场景中的 GameObject
3. 点击 Play 按钮
4. 脚本会自动连接到推理服务并开始接收情感数据

---

## 📊 工作流程

```
ThinkGear 设备
    ↓ (原始 EEG 数据)
数据采集模块 (thinkgear.py)
    ↓ (多模态特征)
特征预处理 (scaler 标准化)
    ↓ (160维特征向量)
模型推理 (multimodal_cnn + cvae)
    ↓ (原始预测概率)
概率聚合器 (ProbabilityAggregator)
    ↓ (平滑后的概率分布)
滑动窗口投票器 (SlidingWindowVoter)
    ↓ (稳定的情感标签 + 过渡进度)
Unity通信模块 (unity_comm.py)
    ↓ (JSON via WebSocket)
EmotionReceiver.cs (Unity端)
    ↓ (天空盒/音乐/特效)
用户视觉体验
```

---

## ⚙️ 配置说明

### 模型配置 (model)

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `path` | 模型权重文件路径 | `models/best_fold4.pt` |
| `type` | 模型类型 | `multimodal_cnn` |
| `device` | 推理设备 | `auto` |
| `modalities` | 特征模态列表 | `[filtered, powerspec, att, med]` |
| `time_steps` | 时间窗口长度 | 10 |
| `use_cvae` | 是否使用CVAE增强 | `true` |

### 投票配置 (voting)

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `window_size` | 滑动窗口大小 | 10 |
| `vote_threshold` | 投票阈值 | 0.6 |
| `transition_duration` | 过渡持续时间(秒) | 1.0 |
| `min_stability_frames` | 最小稳定帧数 | 3 |

### Unity配置 (unity)

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `host` | WebSocket主机 | `localhost` |
| `port` | WebSocket端口 | `8765` |
| `max_connections` | 最大连接数 | 5 |

---

## 🔧 故障排除

### 问题1：无法连接 ThinkGear 设备
```yaml
# 解决方案：使用 mock 模式测试
thinkgear:
  use_mock: true
```

### 问题2：Unity 无法连接到推理服务
- 检查 Python 服务是否正在运行
- 确认端口 8765 未被占用
- 检查防火墙设置

### 问题3：CUDA 内存不足
```yaml
# 解决方案：强制使用 CPU
model:
  device: "cpu"
```

---

## 📈 性能指标

- **推理延迟**: < 100ms (CPU) / < 20ms (GPU)
- **采样率**: 512 Hz (ThinkGear)
- **更新频率**: ~10 Hz (每100ms一次推理)
- **内存占用**: ~500MB (含模型)

---

## 📝 开发日志

- **2026-04-17**: 初始化项目结构，添加 README 和配置文档
- 集成 eeg_modular 训练框架的预训练模型
- 实现 Unity WebSocket 通信桥接

---

## 📚 相关链接

- **上游项目**: [ThirteenAsh/eeg_modular](https://github.com/ThirteenAsh/eeg_modular)
- **Unity AR 项目**: 本项目的父级目录 (`../My project`)
- **ThinkGear 文档**: 参考 `THINKGEAR_SETUP_GUIDE.md`

---

## 📄 许可证

本项目遵循 eeg_modular 项目的开源许可证。具体请参见上游仓库。

---

*最后更新: 2026-04-17*
