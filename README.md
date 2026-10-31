# QuicklyTools

基于 Avalonia 12、.NET 10、CommunityToolkit.Mvvm 和 Microsoft.Extensions.DependencyInjection 的桌面工具箱。

首页以网格展示工具，点击卡片进入工具页面。支持返回上一页和收藏工具；页面切换采用 180 毫秒淡入淡出，保留当前会话中的输入与结果。

- **WiFi 密码查看器**：进入页面自动读取并显示 Windows 本机保存的网络和密码。权限由 Windows WLAN API 控制，读取失败会在页面显示原因。
- **JSON 可视化**：生成 TreeView，支持对象、数组和基本值、格式化、全部展开/收起及解析错误位置提示。格式化保留可读中文，树形视图会解码 Unicode 转义。单次最多 20,000 个节点。

## 结构

| 目录 | 职责 |
| --- | --- |
| DependencyInjection | 应用组合入口、页面映射和工具注册 |
| Models | 工具元数据、WiFi 配置和 JSON 解析结果 |
| Services | 导航、Windows WLAN 调用、JSON 解析 |
| Stores | 首页和收藏共享的工具状态 |
| ViewModels | 可观察状态和命令，工具页面位于 Tools 子目录 |
| Views | XAML 布局；窗口拖动、最小化等视图行为留在 code-behind |

App 创建并在退出时释放容器。页面 ViewModel 和共享 Store 使用单例，视图按需创建。ViewLocator 根据注册映射从容器获取视图，不使用类型名称替换或反射实例化。

导航和 JSON 解析直接注入具体类，不为单一实现单独定义接口。仅 WiFi 读取保留 IWifiProfileService，用于隔离 Windows WLAN API，验证时可替换为模拟数据。

## 新增工具

1. 新建继承 ToolViewModelBase 的 ViewModel，通过构造函数注入服务；使用 ObservableProperty 和 RelayCommand 暴露状态及操作。
2. 新建 UserControl，设置 x:DataType，绑定对应 ViewModel。
3. 在 DependencyInjection/ServiceCollectionExtensions.cs 中注册：

    services.AddTool<MyToolViewModel, MyToolView>(
        "工具名称", "工具说明", "\uE943", "#EAE8FF");

这一条注册同时加入容器、视图映射和首页目录。内部跳转可注入 NavigationService；进入工具页时需要执行操作，可重写 ToolViewModelBase.OnNavigatedTo。工具和收藏目前保存在内存中，重启后重新初始化。

## 构建

    dotnet build QuicklyTools.slnx
