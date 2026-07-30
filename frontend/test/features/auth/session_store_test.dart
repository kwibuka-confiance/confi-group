import 'package:confios/features/auth/data/models/session.dart';
import 'package:confios/features/auth/data/session_store.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  Session sessionValidFor(Duration duration) => Session(
    accessToken: 'token',
    expiresAt: DateTime.now().add(duration),
    userId: 'u1',
    tenantId: 't1',
    businessName: 'KwaConfi Depot',
    fullName: 'Confiance Owner',
    email: 'owner@kwaconfi.rw',
    permissions: const ['catalog.products.read'],
  );

  setUp(() => SharedPreferences.setMockInitialValues({}));

  test('save then load returns the same session', () async {
    const store = SessionStore();
    await store.save(sessionValidFor(const Duration(hours: 1)));

    final restored = await store.load();

    expect(restored, isNotNull);
    expect(restored!.accessToken, 'token');
    expect(restored.businessName, 'KwaConfi Depot');
    expect(restored.permissions, ['catalog.products.read']);
  });

  test('an expired session is discarded on load', () async {
    const store = SessionStore();
    await store.save(sessionValidFor(const Duration(hours: -1)));

    expect(await store.load(), isNull);
  });

  test('clear removes the stored session', () async {
    const store = SessionStore();
    await store.save(sessionValidFor(const Duration(hours: 1)));

    await store.clear();

    expect(await store.load(), isNull);
  });
}
