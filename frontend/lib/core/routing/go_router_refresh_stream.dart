import 'dart:async';

import 'package:flutter/foundation.dart';

/// Bridges a [Stream] (a Bloc/Cubit state stream) to a [Listenable] so GoRouter
/// re-evaluates its redirect whenever the stream emits — e.g. on sign in/out.
class GoRouterRefreshStream extends ChangeNotifier {
  GoRouterRefreshStream(Stream<dynamic> stream) {
    notifyListeners();
    _subscription = stream.asBroadcastStream().listen((_) => notifyListeners());
  }

  late final StreamSubscription<dynamic> _subscription;

  @override
  void dispose() {
    _subscription.cancel();
    super.dispose();
  }
}
