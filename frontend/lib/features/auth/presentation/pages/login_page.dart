import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../bloc/login_bloc.dart';
import '../cubit/session_cubit.dart';
import '../widgets/onboarding_widgets.dart';

/// "Welcome back" — the sign-in screen. On success it hands the session to the
/// [SessionCubit], which flips the app over to the home screen.
class LoginPage extends StatefulWidget {
  const LoginPage({
    super.key,
    this.initialHandle,
    required this.onCreateBusiness,
  });

  final String? initialHandle;
  final VoidCallback onCreateBusiness;

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _handle =
      TextEditingController(text: widget.initialHandle ?? '');
  final _email = TextEditingController();
  final _password = TextEditingController();
  bool _obscure = true;

  @override
  void dispose() {
    _handle.dispose();
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  void _submit(BuildContext context) {
    if (!_formKey.currentState!.validate()) {
      return;
    }
    context.read<LoginBloc>().add(
      LoginSubmitted(
        businessHandle: _handle.text.trim(),
        email: _email.text.trim(),
        password: _password.text,
        locale: Localizations.localeOf(context).languageCode,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return BlocListener<LoginBloc, LoginState>(
      listenWhen: (previous, current) =>
          current.status == LoginStatus.success && current.session != null,
      listener: (context, state) =>
          context.read<SessionCubit>().signIn(state.session!),
      child: Scaffold(
        appBar: AppBar(
          title: Text(l10n.appTitle),
          actions: const [ThemeMenuButton(), SizedBox(width: 4)],
        ),
        body: LayoutBuilder(
          builder: (context, constraints) {
            final wide = constraints.maxWidth >= 900;
            if (!wide) {
              return _form(context, l10n);
            }
            return Row(
              children: [
                const Expanded(flex: 5, child: BrandPanel()),
                Expanded(flex: 6, child: _form(context, l10n)),
              ],
            );
          },
        ),
      ),
    );
  }

  Widget _form(BuildContext context, AppLocalizations l10n) {
    return SafeArea(
      child: LayoutBuilder(
        builder: (context, constraints) {
          return SingleChildScrollView(
            child: ConstrainedBox(
              constraints: BoxConstraints(minHeight: constraints.maxHeight),
              child: Center(
                child: ConstrainedBox(
                  constraints: const BoxConstraints(maxWidth: 460),
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 24,
                      vertical: 32,
                    ),
                    child: BlocBuilder<LoginBloc, LoginState>(
                      builder: (context, state) => _fields(context, l10n, state),
                    ),
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _fields(BuildContext context, AppLocalizations l10n, LoginState state) {
    final banner = _bannerError(l10n, state);
    return Form(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            l10n.signInTitle,
            style: Theme.of(
              context,
            ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
          ),
          const SizedBox(height: 4),
          Text(
            l10n.signInSubtitle,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              color: Theme.of(context).colorScheme.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: 24),
          if (banner != null) ...[
            InfoBanner(message: banner),
            const SizedBox(height: 16),
          ],
          _field(
            controller: _handle,
            label: l10n.handleFieldLabel,
            helper: l10n.signInHandleHelper,
            serverField: 'BusinessHandle',
            state: state,
            icon: Icons.link_outlined,
            l10n: l10n,
          ),
          _field(
            controller: _email,
            label: l10n.emailFieldLabel,
            serverField: 'Email',
            state: state,
            icon: Icons.mail_outline,
            keyboardType: TextInputType.emailAddress,
            l10n: l10n,
          ),
          _field(
            controller: _password,
            label: l10n.passwordFieldLabel,
            serverField: 'Password',
            state: state,
            icon: Icons.lock_outline,
            obscure: _obscure,
            l10n: l10n,
            suffix: IconButton(
              icon: Icon(
                _obscure
                    ? Icons.visibility_outlined
                    : Icons.visibility_off_outlined,
              ),
              tooltip: _obscure ? l10n.showPassword : l10n.hidePassword,
              onPressed: () => setState(() => _obscure = !_obscure),
            ),
          ),
          const SizedBox(height: 12),
          FilledButton(
            onPressed: state.isSubmitting ? null : () => _submit(context),
            child: state.isSubmitting
                ? Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const SizedBox(
                        height: 18,
                        width: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      ),
                      const SizedBox(width: 12),
                      Text(l10n.signingInButton),
                    ],
                  )
                : Text(l10n.signInButton),
          ),
          const SizedBox(height: 16),
          Wrap(
            alignment: WrapAlignment.center,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              Text(
                l10n.noBusinessPrompt,
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              TextButton(
                onPressed: widget.onCreateBusiness,
                child: Text(l10n.createBusinessLink),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _field({
    required TextEditingController controller,
    required String label,
    required String serverField,
    required LoginState state,
    required IconData icon,
    required AppLocalizations l10n,
    String? helper,
    bool obscure = false,
    TextInputType? keyboardType,
    Widget? suffix,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: TextFormField(
        controller: controller,
        obscureText: obscure,
        keyboardType: keyboardType,
        decoration: InputDecoration(
          labelText: label,
          helperText: helper,
          helperMaxLines: 2,
          prefixIcon: Icon(icon),
          suffixIcon: suffix,
          errorText: _serverError(l10n, state, serverField),
        ),
        validator: (value) => (value == null || value.trim().isEmpty)
            ? l10n.fieldRequired
            : null,
      ),
    );
  }

  String? _serverError(AppLocalizations l10n, LoginState state, String field) {
    final codes = state.fieldErrors[field];
    if (codes == null || codes.isEmpty) {
      return null;
    }
    return switch (codes.first) {
      'REQUIRED' => l10n.fieldRequired,
      'INVALID_FORMAT' => l10n.fieldInvalidFormat,
      _ => l10n.fieldInvalidFormat,
    };
  }

  String? _bannerError(AppLocalizations l10n, LoginState state) {
    if (state.status != LoginStatus.failure) {
      return null;
    }
    return switch (state.errorCode) {
      ApiErrorCodes.validationFailed => null,
      ApiErrorCodes.network => l10n.errorNetwork,
      _ => state.errorMessage ?? l10n.errorUnexpected,
    };
  }
}
