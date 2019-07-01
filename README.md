# OpenHaptics4CSharp
### 说明
```
这个项目是 OpenHaptics hd.dll hl.dll for C# API 的项目（ hl.dll for C# 正在开发，Demo示例正在开发）
hd.dll 是触觉式力反馈的低级别的基础层；
hl.dll hl构建在hd之上，是为高级触觉场景渲染而设计的，它针对的是高级OpenGL开发人员，能快速轻松地将触觉力反馈效果添加到现有的图形式应用程序中
```

```
官方有一个针对Unity的插件
https://assetstore.unity.com/packages/tools/integration/3d-systems-openhaptics-unity-plugin-134024
https://softwaresupport.3dsystems.com/knowledgebase/article/KA-01405/en-us
```


### 笔记一
```C++
//这是一个C++文件引入的外部常量，在C#中的引入，使用 WinAPI LoadLibrary 
extern __declspec(dllimport) const char* HL_STIFFNESS;  
```
```C#
IntPtr lib = LoadLibrary(HL_DLL_PATH);
//if error ...
IntPtr ptr = GetProcAddress(lib, varName);
//if error ...
IntPtr cVar = Marshal.ReadIntPtr(ptr);        //in here，常量在程序启动时地址是固定的，在C#中引入C++的外部常量，必须保持地址一致，否则C#运行会出现内存错误
FreeLibrary()
```


### 笔记二
```C++
//hd.dll 其中一个函数，参数是指针 double 类型
//跟据API文档说明，参数名称对应获取其值，返回的值为 double 类型；不同的参数名可能只返回一个 double 数据，也有可能是一组，也有可能是结构数据，SO.
HDAPI void HDAPIENTRY hdGetDoublev(HDenum pname, HDdouble *params); 
```
```C#
//在 C# 中的函数重载方式
[DllImport(HD_DLL_PATH)]
public static extern void hdGetIntegerv(HDGetParameters hdProp, IntPtr value);

[DllImport(HD_DLL_PATH)]
public static extern void hdGetIntegerv(HDGetParameters hdProp, int[] value);

[DllImport(HD_DLL_PATH)]
public static extern void hdGetIntegerv(HDGetParameters hdProp, ref int value);

//还可以重载，但不建议
[DllImport(HD_DLL_PATH)]
public static extern void hdGetIntegerv(Uint32 hdProp, IntPtr value);

[DllImport(HD_DLL_PATH)]
public static extern void hdGetIntegerv(Uint32 hdProp, int[] value);

[DllImport(HD_DLL_PATH)]
public static extern void hdGetIntegerv(Uint32 hdProp, ref int value);
```
