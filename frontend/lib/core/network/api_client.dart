import 'package:dio/dio.dart';

import 'api_exception.dart';

/// Thin wrapper over Dio that speaks the ConfiOS envelope: it unwraps the
/// `data` object on success and translates the error envelope into an
/// [ApiException] on failure, so callers never touch transport details.
class ApiClient {
  ApiClient(this._dio);

  factory ApiClient.create(String baseUrl) {
    final dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 10),
        headers: const {'Accept': 'application/json'},
        // 4xx carries the error envelope we want to read, so do not let Dio
        // throw on it; only 5xx and transport failures raise a DioException.
        validateStatus: (status) => status != null && status < 500,
      ),
    );
    return ApiClient(dio);
  }

  final Dio _dio;

  /// POSTs [body] to [path] and returns the unwrapped `data` object.
  Future<Map<String, dynamic>> post(
    String path, {
    Object? body,
    String? locale,
  }) async {
    try {
      final response = await _dio.post<dynamic>(
        path,
        data: body,
        options: Options(
          headers: {if (locale != null) 'Accept-Language': locale},
        ),
      );

      final data = response.data;
      final map = data is Map<String, dynamic> ? data : const <String, dynamic>{};
      final status = response.statusCode ?? 0;

      if (status >= 400) {
        throw _toApiException(map, status);
      }

      final payload = map['data'];
      return payload is Map<String, dynamic> ? payload : <String, dynamic>{};
    } on DioException catch (error) {
      final response = error.response;
      if (response?.data is Map<String, dynamic>) {
        throw _toApiException(
          response!.data as Map<String, dynamic>,
          response.statusCode ?? 0,
        );
      }
      throw ApiException(
        code: ApiErrorCodes.network,
        message: error.message ?? 'Network error',
      );
    }
  }

  ApiException _toApiException(Map<String, dynamic> body, int statusCode) {
    final failures = <String, List<String>>{};
    final details = body['details'];
    if (details is Map<String, dynamic> && details['failures'] is Map) {
      (details['failures'] as Map).forEach((key, value) {
        if (value is List) {
          failures[key.toString()] = value.map((e) => e.toString()).toList();
        }
      });
    }

    return ApiException(
      code: body['code']?.toString() ?? ApiErrorCodes.unexpected,
      message: body['message']?.toString() ?? 'Unexpected error',
      fieldErrors: failures,
      statusCode: statusCode,
      traceId: body['traceId']?.toString(),
    );
  }
}
