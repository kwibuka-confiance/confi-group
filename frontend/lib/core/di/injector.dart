import 'package:get_it/get_it.dart';

import '../../features/auth/data/auth_repository.dart';
import '../../features/catalog/data/product_repository.dart';
import '../config/app_config.dart';
import '../network/api_client.dart';

/// Service locator. Wiring lives here rather than in widgets so the composition
/// root is a single, testable place.
final GetIt sl = GetIt.instance;

void configureDependencies() {
  sl
    ..registerLazySingleton<ApiClient>(() => ApiClient.create(AppConfig.apiBaseUrl))
    ..registerLazySingleton<AuthRepository>(() => AuthRepository(sl<ApiClient>()))
    ..registerLazySingleton<ProductRepository>(() => ProductRepository(sl<ApiClient>()));
}
