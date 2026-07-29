import '../../../core/network/api_client.dart';
import 'models/provision_tenant_request.dart';
import 'models/provision_tenant_result.dart';
import 'models/session.dart';

/// The Identity module's client-side surface: provisioning a new business and
/// signing in to an existing one.
class AuthRepository {
  const AuthRepository(this._client);

  final ApiClient _client;

  Future<ProvisionTenantResult> provisionTenant(
    ProvisionTenantRequest request, {
    String? locale,
  }) async {
    final data = await _client.post(
      '/api/v1/tenants',
      body: request.toJson(),
      locale: locale,
    );
    return ProvisionTenantResult.fromJson(data);
  }

  Future<Session> login({
    required String businessHandle,
    required String email,
    required String password,
    String? locale,
  }) async {
    final data = await _client.post(
      '/api/v1/auth/login',
      body: {
        'businessHandle': businessHandle,
        'email': email,
        'password': password,
      },
      locale: locale,
    );
    return Session.fromJson(data);
  }
}
