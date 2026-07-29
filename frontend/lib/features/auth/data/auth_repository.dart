import '../../../core/network/api_client.dart';
import 'models/provision_tenant_request.dart';
import 'models/provision_tenant_result.dart';

/// The Identity module's client-side surface. For now it provisions a new
/// business; sign-in lands here once the backend issues tokens.
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
}
