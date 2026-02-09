使用的依赖：
1. odin：编辑器使用
2. YooAsset：资源加载、管理、热更新
3. InputSystem
<!-- luban：配表 -->
<!-- HybridCLR：代码热更新 -->

程序集：
1. AOT程序设置自动引用，热更新程序集不能设置自动引用


程序集介绍：
1. DataTableHot：excel数据表类热更新
2. GameFramework.Hot：框架热更新
3. GameFramework.Log：日志类
4. Project：业务代码热更新


配置数据：
1. launch脚本
2. GameConfig.asset配置文件


打包流程：
1. HybridCLR生成全部
2. 拷贝AOTGenericReferences到自己的程序集下并且添加对link.xml里类的引用
3. HybridCLR编译
4. 把AOTGenericReferences里定义的被裁剪的dll拷贝到StreamingAssets下
5. YooAsset构建
6. 构建游戏






单机时：
1. 开启GameFramework.Hot的Auto Referenced，让默认程序集可以引用里面的类



使用：
1. Tools里打开日志级别
2. 创建配置文件Assets/Content/Config/GameConfig.asset，创建输入文件Assets/Content/Config/PlayerInputAsset.inputactions，把这两个文件加到YooAsset打包里





<!-- 
Cinemachine
Quantum Console控制台
DOTween Pro提供插值方法
-->