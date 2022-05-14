// pch.cpp: файл исходного кода, соответствующий предварительно скомпилированному заголовочному файлу

#include "pch.h"

// При использовании предварительно скомпилированных заголовочных файлов необходим следующий файл исходного кода для выполнения сборки.

extern "C" _declspec(dllexport) double Interpolate(const int len, const int uniform_len, const double* points, const double* values, double* results)
{
	DFTaskPtr task;
	int i = 3;
	double* scoeff = new double[(len - 1) * DF_PP_CUBIC];
	MKL_INT dorder[] = { 1 , 1 , 1 };
	double segment[] = { points[0],  points[len - 1] };

	int status = dfdNewTask1D(&task, len, points, DF_NON_UNIFORM_PARTITION, 1, values, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.1;

	status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_FREE_END, NULL, DF_NO_IC, NULL, scoeff, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.2;

	status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
	if (status != DF_STATUS_OK) return status + 0.3;

	status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, uniform_len, segment, DF_UNIFORM_PARTITION, i, dorder, NULL, results, DF_NO_HINT, NULL);
	if (status != DF_STATUS_OK) return status + 0.4;

	status = dfDeleteTask(&task);
	if (status != DF_STATUS_OK) return status + 0.5;

	return status;
}

extern "C" _declspec(dllexport) double Integrate(const int len, const double* points, const double* values, const double* limits, double* integrals)
{
	DFTaskPtr task;
	double* scoeff = new double[(len - 1) * DF_PP_CUBIC];
	double* limit1 = new double[1];
	double* limit2 = new double[1];
	limit1[0] = limits[0];
	limit2[0] = limits[1];

	int status = dfdNewTask1D(&task, len, points, DF_NON_UNIFORM_PARTITION, 1, values, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.1;

	status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_FREE_END, NULL, DF_NO_IC, NULL, scoeff, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.2;

	status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
	if (status != DF_STATUS_OK) return status + 0.3;

	status = dfdIntegrate1D(task, DF_METHOD_PP, 1, limit1, DF_UNIFORM_PARTITION, limit2, DF_UNIFORM_PARTITION, NULL, NULL, integrals, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.4;

	status = dfDeleteTask(&task);
	if (status != DF_STATUS_OK) return status + 0.5;

	return status;
}