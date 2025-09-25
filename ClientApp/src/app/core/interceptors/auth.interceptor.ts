import {
  HttpErrorResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.getAccessToken();
  let authReq = req;

  if (accessToken && !req.url.endsWith('/auth/login') && !req.url.endsWith('/auth/register')) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  }

  return next(authReq).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        const refreshToken = authService.getRefreshToken();
        if (refreshToken) {
          return authService.refreshToken(refreshToken).pipe(
            switchMap(() => {
              const newToken = authService.getAccessToken();
              if (!newToken) {
                authService.logout();
                return throwError(() => error);
              }

              const retryRequest = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newToken}`,
                },
              });
              return next(retryRequest);
            }),
            catchError((refreshError) => {
              authService.logout();
              return throwError(() => refreshError);
            })
          );
        }

        authService.logout();
      }

      return throwError(() => error);
    })
  );
};
