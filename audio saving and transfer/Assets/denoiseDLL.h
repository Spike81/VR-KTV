#ifdef __cplusplus
extern "C" {
#endif

#ifdef DENOISE_FUNC
#else
#define DENOISE_FUNC _declspec(dllimport)//当编译时，头文件不参加编译，所以.cpp文件中先定义，后头文件被包含进来，因此外部使用时，为dllexport，而在内部编译时，则为dllimport
#endif

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include "stb_fft.h"

//绝对值加法
int DENOISE_FUNC denoise(char in_files[], char out_files[]);

class DENOISE_FUNC Denoiser
{
public:

	typedef struct
	{
		int frameSize;
		int windowSize;
		int freq_size;
		int sampleRate;
		float *windowing;
		stb_fft_real_plan *realPlan;
		float *fifo;
		float *synthesis_mem;
		float *smooth_mem;
		cmplx *samples;
		float *noise_mem;
		int noise_count;
	} SimpleDenoiseHandle;

	Denoiser();

	~Denoiser();

	void wavWrite_f32(char *filename, float *buffer, int sampleRate, uint32_t totalSampleCount, uint32_t channels);
	void splitpath(const char * path, char * drv, char * dir, char * name, char * ext);
	void SimpleDenoise_Free(SimpleDenoiseHandle * handle);
	int SimpleDenoise_Init(SimpleDenoiseHandle * handle, size_t sampleRate, size_t ms);
	int Simple_NoiseEstimator(SimpleDenoiseHandle * handle, const float * input, int Sampling);
	int SimpleDenoise_Proc(SimpleDenoiseHandle * handle, const float * input, float * output);
	float* wavRead_f32(const char *filename, uint32_t *sampleRate, uint64_t *sampleCount, uint32_t *channels);
	void simpleDenoise(char *in_file, char *out_file);
};

#ifdef __cplusplus
}
#endif