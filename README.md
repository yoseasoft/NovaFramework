## Nova Framework

引擎针对==Logger==日志输出的格式化方式进行了封装，在内部确定调用后再进行转译，这样有效避免了在外部可能输出不会调用的情况下，仍然对参数进行解析的无效操作。  
封装的格式化标签及其功能如下：  
- %d - 基础数据类型输出  
- %s - 字符串数据类型输出  
- %o - 对象实例类型输出（ToString）  
- %p - 对象实例指针地址输出  
- %t - 对象实例的类类型输出  
- %i - 对象实例信息输出（ToString）  
- %f - 类类型的完整名称输出  
- %v - 容器实例内部元素输出  

使用方式如下：
```c#
TestObject obj = new TestObject();
Debugger.Info("测试类型显示名称{%f}", typeof(TestObject));
Debugger.Info("测试对象显示名称{%t}", obj);
```

[打开说明文档](Documentation/index.md)
