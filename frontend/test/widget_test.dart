import 'package:confios/app/app.dart';
import 'package:confios/core/di/injector.dart';
import 'package:confios/features/auth/data/models/session.dart';
import 'package:confios/features/auth/data/session_store.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  setUp(() async {
    SharedPreferences.setMockInitialValues({});
    await sl.reset();
    configureDependencies();
  });

  testWidgets('opens on the sign-in screen when signed out', (tester) async {
    await tester.pumpWidget(const ConfiOsApp());
    await tester.pumpAndSettle();

    // No persisted session, so the router redirects to the sign-in route.
    expect(find.text('Welcome back'), findsOneWidget);
    expect(find.text('Sign in'), findsOneWidget);
    expect(find.text('Create a business'), findsOneWidget);
  });

  testWidgets('restores a saved session straight to home', (tester) async {
    final session = Session(
      accessToken: 'token',
      expiresAt: DateTime.now().add(const Duration(hours: 1)),
      userId: 'u1',
      tenantId: 't1',
      businessName: 'KwaConfi Depot',
      fullName: 'Confiance Owner',
      email: 'owner@kwaconfi.rw',
      permissions: const [],
    );
    await sl<SessionStore>().save(session);
    final restored = await sl<SessionStore>().load();

    await tester.pumpWidget(ConfiOsApp(initialSession: restored));
    await tester.pumpAndSettle();

    // Redirected past sign-in to the home screen.
    expect(find.text('Open catalog'), findsOneWidget);
    expect(find.text('Welcome back'), findsNothing);
  });
}
