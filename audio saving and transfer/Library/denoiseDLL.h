#ifdef DENOISE_FUNC
#else
#define DENOISE_FUNC _declspec(dllimport)//������ʱ��ͷ�ļ����μӱ��룬����.cpp�ļ����ȶ��壬��ͷ�ļ�����������������ⲿʹ��ʱ��Ϊdllexport�������ڲ�����ʱ����Ϊdllimport
#endif

//����ֵ�ӷ�
int DENOISE_FUNC denoise(char in_files[], char out_files[]);


