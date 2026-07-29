import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/provision_tenant_request.dart';
import '../bloc/sign_up_bloc.dart';

/// "Create your business" — the sign-up form that provisions a tenant against
/// the backend. The widget only collects input and renders [SignUpState]; the
/// work lives in [SignUpBloc].
class SignUpPage extends StatefulWidget {
  const SignUpPage({super.key});

  @override
  State<SignUpPage> createState() => _SignUpPageState();
}

class _SignUpPageState extends State<SignUpPage> {
  final _formKey = GlobalKey<FormState>();

  final _name = TextEditingController();
  final _slug = TextEditingController();
  final _countryCode = TextEditingController(text: 'RW');
  final _currencyCode = TextEditingController(text: 'RWF');
  final _timeZoneId = TextEditingController(text: 'Africa/Kigali');
  final _branchName = TextEditingController(text: 'Main Branch');
  final _ownerName = TextEditingController();
  final _ownerEmail = TextEditingController();
  final _ownerPassword = TextEditingController();

  String _defaultLanguage = 'en';

  @override
  void dispose() {
    for (final controller in [
      _name,
      _slug,
      _countryCode,
      _currencyCode,
      _timeZoneId,
      _branchName,
      _ownerName,
      _ownerEmail,
      _ownerPassword,
    ]) {
      controller.dispose();
    }
    super.dispose();
  }

  void _submit(BuildContext context) {
    if (!_formKey.currentState!.validate()) {
      return;
    }
    final request = ProvisionTenantRequest(
      name: _name.text.trim(),
      slug: _slug.text.trim(),
      countryCode: _countryCode.text.trim(),
      currencyCode: _currencyCode.text.trim(),
      defaultLanguage: _defaultLanguage,
      timeZoneId: _timeZoneId.text.trim(),
      ownerEmail: _ownerEmail.text.trim(),
      ownerFullName: _ownerName.text.trim(),
      ownerPassword: _ownerPassword.text,
      firstBranchName: _branchName.text.trim(),
    );
    context.read<SignUpBloc>().add(
      SignUpSubmitted(
        request,
        locale: Localizations.localeOf(context).languageCode,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Scaffold(
      appBar: AppBar(title: Text(l10n.appTitle)),
      body: BlocBuilder<SignUpBloc, SignUpState>(
        builder: (context, state) {
          if (state.status == SignUpStatus.success && state.result != null) {
            return _SuccessView(
              tenantId: state.result!.tenantId,
              onReset: () => context.read<SignUpBloc>().add(const SignUpReset()),
            );
          }
          return _FormView(
            formKey: _formKey,
            state: state,
            name: _name,
            slug: _slug,
            countryCode: _countryCode,
            currencyCode: _currencyCode,
            timeZoneId: _timeZoneId,
            branchName: _branchName,
            ownerName: _ownerName,
            ownerEmail: _ownerEmail,
            ownerPassword: _ownerPassword,
            defaultLanguage: _defaultLanguage,
            onLanguageChanged: (value) =>
                setState(() => _defaultLanguage = value),
            onSubmit: () => _submit(context),
          );
        },
      ),
    );
  }
}

class _FormView extends StatelessWidget {
  const _FormView({
    required this.formKey,
    required this.state,
    required this.name,
    required this.slug,
    required this.countryCode,
    required this.currencyCode,
    required this.timeZoneId,
    required this.branchName,
    required this.ownerName,
    required this.ownerEmail,
    required this.ownerPassword,
    required this.defaultLanguage,
    required this.onLanguageChanged,
    required this.onSubmit,
  });

  final GlobalKey<FormState> formKey;
  final SignUpState state;
  final TextEditingController name;
  final TextEditingController slug;
  final TextEditingController countryCode;
  final TextEditingController currencyCode;
  final TextEditingController timeZoneId;
  final TextEditingController branchName;
  final TextEditingController ownerName;
  final TextEditingController ownerEmail;
  final TextEditingController ownerPassword;
  final String defaultLanguage;
  final ValueChanged<String> onLanguageChanged;
  final VoidCallback onSubmit;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final banner = _bannerError(l10n, state);

    return Center(
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 480),
          child: Form(
            key: formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  l10n.signUpTitle,
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
                const SizedBox(height: 4),
                Text(
                  l10n.signUpSubtitle,
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
                const SizedBox(height: 20),
                if (banner != null) ...[
                  _ErrorBanner(message: banner),
                  const SizedBox(height: 16),
                ],
                _field(
                  context,
                  controller: name,
                  label: l10n.businessNameLabel,
                  serverField: 'Name',
                ),
                _field(
                  context,
                  controller: slug,
                  label: l10n.slugLabel,
                  helper: l10n.slugHelper,
                  serverField: 'Slug',
                ),
                Row(
                  children: [
                    Expanded(
                      child: _field(
                        context,
                        controller: countryCode,
                        label: l10n.countryCodeLabel,
                        serverField: 'CountryCode',
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: _field(
                        context,
                        controller: currencyCode,
                        label: l10n.currencyCodeLabel,
                        serverField: 'CurrencyCode',
                      ),
                    ),
                  ],
                ),
                _LanguageDropdown(
                  value: defaultLanguage,
                  onChanged: onLanguageChanged,
                ),
                const SizedBox(height: 12),
                _field(
                  context,
                  controller: timeZoneId,
                  label: l10n.timeZoneLabel,
                  serverField: 'TimeZoneId',
                ),
                _field(
                  context,
                  controller: branchName,
                  label: l10n.firstBranchNameLabel,
                  serverField: 'FirstBranchName',
                ),
                const Divider(height: 32),
                _field(
                  context,
                  controller: ownerName,
                  label: l10n.ownerFullNameLabel,
                  serverField: 'OwnerFullName',
                ),
                _field(
                  context,
                  controller: ownerEmail,
                  label: l10n.ownerEmailLabel,
                  keyboardType: TextInputType.emailAddress,
                  serverField: 'OwnerEmail',
                ),
                _field(
                  context,
                  controller: ownerPassword,
                  label: l10n.ownerPasswordLabel,
                  obscure: true,
                  serverField: 'OwnerPassword',
                ),
                const SizedBox(height: 24),
                FilledButton(
                  onPressed: state.isSubmitting ? null : onSubmit,
                  child: Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
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
                              Text(l10n.creatingButton),
                            ],
                          )
                        : Text(l10n.createButton),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _field(
    BuildContext context, {
    required TextEditingController controller,
    required String label,
    required String serverField,
    String? helper,
    bool obscure = false,
    TextInputType? keyboardType,
  }) {
    final l10n = AppLocalizations.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: TextFormField(
        controller: controller,
        obscureText: obscure,
        keyboardType: keyboardType,
        autovalidateMode: AutovalidateMode.disabled,
        decoration: InputDecoration(
          labelText: label,
          helperText: helper,
          errorText: _serverFieldError(l10n, state, serverField),
        ),
        validator: (value) =>
            (value == null || value.trim().isEmpty) ? l10n.fieldRequired : null,
      ),
    );
  }
}

class _LanguageDropdown extends StatelessWidget {
  const _LanguageDropdown({required this.value, required this.onChanged});

  final String value;
  final ValueChanged<String> onChanged;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return DropdownButtonFormField<String>(
      initialValue: value,
      decoration: InputDecoration(labelText: l10n.defaultLanguageLabel),
      items: [
        DropdownMenuItem(value: 'en', child: Text(l10n.languageEnglish)),
        DropdownMenuItem(value: 'rw', child: Text(l10n.languageKinyarwanda)),
        DropdownMenuItem(value: 'fr', child: Text(l10n.languageFrench)),
      ],
      onChanged: (v) => onChanged(v ?? 'en'),
    );
  }
}

class _ErrorBanner extends StatelessWidget {
  const _ErrorBanner({required this.message});

  final String message;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: scheme.errorContainer,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        children: [
          Icon(Icons.error_outline, color: scheme.onErrorContainer),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              message,
              style: TextStyle(color: scheme.onErrorContainer),
            ),
          ),
        ],
      ),
    );
  }
}

class _SuccessView extends StatelessWidget {
  const _SuccessView({required this.tenantId, required this.onReset});

  final String tenantId;
  final VoidCallback onReset;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    return Center(
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 480),
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.check_circle, size: 64, color: scheme.primary),
              const SizedBox(height: 16),
              Text(
                l10n.successTitle,
                style: Theme.of(context).textTheme.headlineSmall,
              ),
              const SizedBox(height: 8),
              Text(
                l10n.successBody(tenantId),
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              const SizedBox(height: 24),
              OutlinedButton(onPressed: onReset, child: Text(l10n.startOver)),
            ],
          ),
        ),
      ),
    );
  }
}

String? _serverFieldError(
  AppLocalizations l10n,
  SignUpState state,
  String field,
) {
  final codes = state.fieldErrors[field];
  if (codes == null || codes.isEmpty) {
    return null;
  }
  return switch (codes.first) {
    'REQUIRED' => l10n.fieldRequired,
    'INVALID_FORMAT' => l10n.fieldInvalidFormat,
    'TOO_SHORT' => l10n.fieldTooShort,
    'TOO_LONG' => l10n.fieldTooLong,
    _ => l10n.fieldInvalidFormat,
  };
}

String? _bannerError(AppLocalizations l10n, SignUpState state) {
  if (state.status != SignUpStatus.failure) {
    return null;
  }
  return switch (state.errorCode) {
    ApiErrorCodes.validationFailed => null, // shown inline per field
    ApiErrorCodes.tenantSlugTaken => l10n.errorSlugTaken,
    ApiErrorCodes.network => l10n.errorNetwork,
    _ => state.errorMessage ?? l10n.errorUnexpected,
  };
}
