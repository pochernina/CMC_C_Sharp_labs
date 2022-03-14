// pch.cpp: файл исходного кода, соответствующий предварительно скомпилированному заголовочному файлу

#include "pch.h"
extern "C" _declspec(dllexport) double Spline(const double* values, const int nx, const int ny, const double step, double* deriv)
{
	DFTaskPtr task;
	double x[] = {0, (nx - 1) * step};
	int i = 2;
	double* scoeff = new double[ny * (nx - 1) * DF_PP_CUBIC];

	int status = dfdNewTask1D(&task, nx, x, DF_UNIFORM_PARTITION, ny, values, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.1;
	status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_FREE_END, NULL, DF_NO_IC, NULL, scoeff, DF_NO_HINT);
	if (status != DF_STATUS_OK) return status + 0.2;
	status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
	if (status != DF_STATUS_OK) return status + 0.3;
	MKL_INT dorder[] = {0, 1};
	status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, nx, x, DF_UNIFORM_PARTITION, i, dorder, NULL, deriv, DF_NO_HINT, NULL);
	if (status != DF_STATUS_OK) return status + 0.4;
	status = dfDeleteTask(&task);
	if (status != DF_STATUS_OK) return status + 0.5;

	return status;
}