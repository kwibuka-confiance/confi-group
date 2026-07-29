import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

/// Holds the chosen [ThemeMode] (system / light / dark). Kept in memory for now;
/// persisting the choice across launches is a small follow-up.
class ThemeCubit extends Cubit<ThemeMode> {
  ThemeCubit() : super(ThemeMode.system);

  void select(ThemeMode mode) => emit(mode);
}
