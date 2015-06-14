# UnityTplAndAwait
## 概述
unity3d 中实现 tpl 和使用 await/async 关键字。

## 说明
await/async 虽然是 .net 4.5 加入的，但是相对于 .net 3.5 IL 层面没有加入新的指令，可以通过库的方式直接模拟，前提是编译器要支持 await/async 关键字。

unity3d 的 gmcs 版本很低且修改过，不支持 await 关键字，直接把代码扔进 unity 编辑器会不识别。需要使用 vs 编译，或者安装新版本的 [mdk](http://www.mono-project.com/download/)，再找到设置中.net runtimes 选项修改 mdk 版本和修改工程的 sdk 版本，最后把代码编译成 dll，这是使用上的最大限制。

这里的代码是直接从 [mono 主干](https://github.com/mono/mono)上修改而来的，[另外一个项目](https://github.com/OmerMor/AsyncBridge)经过简单测试也可以实现同样的效果，代码量更少，但我没有测试过 AOT 的兼容性，毕竟不是专门为 unity 写的。

## 问题
在移植过程中，AOT 相关的，除了常规的[泛型推导](http://developer.xamarin.com/guides/ios/advanced_topics/limitations/#.NET.c2.a0API.c2.a0Limitations)和原子操作BUG（[AotInterlocked.cs](https://github.com/nswutong/UnityTplAndAwait/blob/master/System.Threading/System.Threading/AotInterlocked.cs)），还碰到了一个非常奇怪的事情。

ThreadPool.UnsafeQueueUserWorkItem 这个接口有一定概率触发 AOT 失败，什么叫一定概率呢？就是第一遍编译 AOT 失败，把含有这个函数的文件移除再加入，AOT 可能就成功了。换句话说，和 msbuild 配置里面的文件包含顺序有关。

这里图省事所有代码都采用了 ThreadPool.QueueUserWorkItem 这个版本。

UnitySynchronizationContext 是随便写的，测试使用。

使用上目前碰到最大的问题是调试不方便。

## 最后
本项目只是作为测试 unity3d 使用 await/async 的可行性，没有经过更多的测试。

其实 unity 上常规使用 await/async，可能更在乎的是异步逻辑，而 tpl 中的并发相关，用处不大，可以精简掉。

Unity 支持 yield 出去一个 Coroutine 对象，也就是 python yield from 的功能，这个和 await/async 已经非常接近了，更推荐使用这种方案。
