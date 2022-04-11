// pch.cpp: файл исходного кода, соответствующий предварительно скомпилированному заголовочному файлу

#include "pch.h"

using namespace std;
using namespace std::chrono;

class Timer
{
private:
	using clock = high_resolution_clock;
	using duration = duration< double, ratio<1> >;
	time_point<clock> point;
public:
	Timer(): point(clock::now()) {}
	void reset() { point = clock::now(); }
	double elapsed() const { return duration_cast<duration>(clock::now() - point).count(); }
};

extern "C" _declspec(dllexport) double MKL_function(const int len, const double* args, const int VMf, double* res_ha, double* res_ep, double* res_without_mkl, double* results)
{
	long long mode = VML_HA;
	int n = len;
	Timer t;
	if (VMf == 0) // vmdTan
	{
		vmdtan(&n, args, res_ha, &mode);
		results[0] = t.elapsed();

		mode = VML_EP;
		t.reset();
		vmdtan(&n, args, res_ep, &mode);
		results[1] = t.elapsed();

		t.reset();
		for (int i = 0; i < len; ++i)
		{
			res_without_mkl[i] = tan(args[i] * 3.1415 / 180.0);
		}
		results[2] = t.elapsed();
		return 0;
	}
	else if (VMf == 1) // vmdErfInv
	{
		vmdErfInv(n, args, res_ha, mode);
		results[0] = t.elapsed();

		mode = VML_EP;
		t.reset();
		vmdErfInv(n, args, res_ep, mode);
		results[1] = t.elapsed();

		t.reset();
		for (int i = 0; i < len; ++i)
		{
			res_without_mkl[i] = 1 / erf(args[i]);
		}
		results[2] = t.elapsed();
		return 0;
	}
	else return -1;
}

// При использовании предварительно скомпилированных заголовочных файлов необходим следующий файл исходного кода для выполнения сборки.
