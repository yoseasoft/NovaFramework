## Object Lifecycle Sample

该示例主要演示引擎内部提供的实体对象所具备的生命周期及其调度流程；  

引擎提供的Bean对象生命周期流程为：
- Initialize 对象初始化，主要由缓存或内部流程管理，用于引擎内部进行对象上下文初始化
- Startup 对象开启，主要由缓存或内部流程管理，用于引擎内部在对象完成初始化之后进行一些后续逻辑设定
- Awake 对象唤醒，由外部业务管理，在对象创建后被立即调用
- Start 对象启动，由外部业务管理，将在对象创建后的下一帧被调用
- Update 对象刷新，由外部业务管理，将在对象创建后的下一帧开始，循环调用
- LateUpdate 对象延迟刷新，由外部业务管理，将在对象创建后的下一帧开始，循环调用
- Execute 对象逻辑刷新，由外部业务管理，与Update的区别是非逐帧调用
- LateExecute 对象逻辑延迟刷新，由外部业务管理，与LateUpdate的区别是非逐帧调用
- Destroy 对象销毁，由外部业务管理，将在当前帧结束后统一调用
- Shutdown 对象关闭，主要由缓存或内部流程管理，用于在对象清理前完成一些关联逻辑解除
- Cleanup 对象清理，主要由缓存或内部流程管理，用于引擎内部进行对象上下文清理

