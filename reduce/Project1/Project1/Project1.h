//MyDLL.h
#ifdef DLL_EXPORT //如果在工程里已经添加预定义宏      
#define MYDLL_EXPORT extern "C" __declspec(dllexport) //那DLLEXPORT就指代__declspec(dllexport) 用于dll的导出#else  //当工程中不包含DLL_EXPORTS预定义时      
#define MYDLL_EXPORT extern "C" __declspec(dllimport) //DLLEXPORT 就指代__declspec(dllimport)
#endif//extern "C"的主要作用就是为了能够正确实现C++代码调用其他C语言代码
DLL_EXPORT int add(int a, int b);

