#ifdef DENOISE_FUNC
#else
#define DENOISE_FUNC _declspec(dllimport)//当编译时，头文件不参加编译，所以.cpp文件中先定义，后头文件被包含进来，因此外部使用时，为dllexport，而在内部编译时，则为dllimport
#endif

//绝对值加法
int DENOISE_FUNC denoise(char in_files[], char out_files[]);


