import 'package:confios/app/app.dart';
import 'package:confios/core/di/injector.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  setUp(() async {
    await sl.reset();
    configureDependencies();
  });

  testWidgets('opens on the sign-in screen when signed out', (tester) async {
    await tester.pumpWidget(const ConfiOsApp());
    await tester.pumpAndSettle();

    // English is the default test locale.
    expect(find.text('Welcome back'), findsOneWidget);
    expect(find.text('Sign in'), findsOneWidget);
    expect(find.text('Create a business'), findsOneWidget);
  });
}
