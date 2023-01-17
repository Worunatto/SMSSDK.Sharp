# SMSSDK.Sharp

## 使用方法 
安装 Nuget 包 `SMSSDK.Sharp`，然后

```csharp
SMSSDK.InitSDK("YourAppKey", "YourAppSecret");
SMSSDK.GetVerificationCode("86", "12388889999");
```

## 注意
- SMSSDK.InitSDK 全局只需要调用一次
- 只实现了 GetVerificationCode 方法，如有需求请提 Issue
