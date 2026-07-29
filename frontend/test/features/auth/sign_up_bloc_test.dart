import 'package:bloc_test/bloc_test.dart';
import 'package:confios/core/network/api_exception.dart';
import 'package:confios/features/auth/data/auth_repository.dart';
import 'package:confios/features/auth/data/models/provision_tenant_request.dart';
import 'package:confios/features/auth/data/models/provision_tenant_result.dart';
import 'package:confios/features/auth/presentation/bloc/sign_up_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

class _MockAuthRepository extends Mock implements AuthRepository {}

class _FakeRequest extends Fake implements ProvisionTenantRequest {}

void main() {
  late AuthRepository repository;

  const request = ProvisionTenantRequest(
    name: 'KwaConfi Depot',
    slug: 'kwaconfi-depot',
    countryCode: 'RW',
    currencyCode: 'RWF',
    defaultLanguage: 'en',
    timeZoneId: 'Africa/Kigali',
    ownerEmail: 'owner@kwaconfi.rw',
    ownerFullName: 'Confiance Owner',
    ownerPassword: 'Str0ngPass!23',
    firstBranchName: 'Main Branch',
  );

  setUpAll(() => registerFallbackValue(_FakeRequest()));
  setUp(() => repository = _MockAuthRepository());

  blocTest<SignUpBloc, SignUpState>(
    'emits [submitting, success] when provisioning succeeds',
    setUp: () {
      when(
        () => repository.provisionTenant(any(), locale: any(named: 'locale')),
      ).thenAnswer(
        (_) async => const ProvisionTenantResult(
          tenantId: 't1',
          branchId: 'b1',
          ownerUserId: 'u1',
        ),
      );
    },
    build: () => SignUpBloc(repository),
    act: (bloc) => bloc.add(const SignUpSubmitted(request)),
    expect: () => [
      const SignUpState(status: SignUpStatus.submitting),
      isA<SignUpState>()
          .having((s) => s.status, 'status', SignUpStatus.success)
          .having((s) => s.result?.tenantId, 'tenantId', 't1'),
    ],
  );

  blocTest<SignUpBloc, SignUpState>(
    'emits [submitting, failure] carrying the slug-taken code',
    setUp: () {
      when(
        () => repository.provisionTenant(any(), locale: any(named: 'locale')),
      ).thenThrow(
        const ApiException(
          code: ApiErrorCodes.tenantSlugTaken,
          message: 'That business handle is already in use.',
        ),
      );
    },
    build: () => SignUpBloc(repository),
    act: (bloc) => bloc.add(const SignUpSubmitted(request)),
    expect: () => [
      const SignUpState(status: SignUpStatus.submitting),
      isA<SignUpState>()
          .having((s) => s.status, 'status', SignUpStatus.failure)
          .having((s) => s.errorCode, 'code', ApiErrorCodes.tenantSlugTaken),
    ],
  );
}
