import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/provision_tenant_request.dart';
import '../bloc/sign_up_bloc.dart';
import '../widgets/onboarding_widgets.dart';

/// Guided sign-up: a four-step wizard (business → region → account → review)
/// that provisions a tenant. The widget collects input and renders state; the
/// work lives in [SignUpBloc].
class SignUpPage extends StatefulWidget {
  const SignUpPage({super.key, this.onSignIn});

  /// Called from the success screen with the new business handle, so the shell
  /// can move the user to sign-in.
  final void Function(String handle)? onSignIn;

  @override
  State<SignUpPage> createState() => _SignUpPageState();
}

class _SignUpPageState extends State<SignUpPage> {
  static const int _reviewIndex = 3;

  final _businessFormKey = GlobalKey<FormState>();
  final _regionFormKey = GlobalKey<FormState>();
  final _accountFormKey = GlobalKey<FormState>();

  final _name = TextEditingController();
  final _slug = TextEditingController();
  final _branchName = TextEditingController(text: 'Main Branch');
  final _countryCode = TextEditingController(text: 'RW');
  final _currencyCode = TextEditingController(text: 'RWF');
  final _timeZoneId = TextEditingController(text: 'Africa/Kigali');
  final _ownerName = TextEditingController();
  final _ownerEmail = TextEditingController();
  final _ownerPassword = TextEditingController();

  int _step = 0;
  String _defaultLanguage = 'en';
  bool _slugEdited = false;
  bool _obscurePassword = true;

  @override
  void dispose() {
    for (final c in [
      _name,
      _slug,
      _branchName,
      _countryCode,
      _currencyCode,
      _timeZoneId,
      _ownerName,
      _ownerEmail,
      _ownerPassword,
    ]) {
      c.dispose();
    }
    super.dispose();
  }

  String _slugify(String input) => input
      .toLowerCase()
      .trim()
      .replaceAll(RegExp(r'[^a-z0-9]+'), '-')
      .replaceAll(RegExp(r'^-+|-+$'), '');

  GlobalKey<FormState>? _formKeyFor(int step) => switch (step) {
    0 => _businessFormKey,
    1 => _regionFormKey,
    2 => _accountFormKey,
    _ => null,
  };

  void _next() {
    final key = _formKeyFor(_step);
    if (key != null && !(key.currentState?.validate() ?? true)) {
      return;
    }
    if (_step < _reviewIndex) {
      setState(() => _step++);
    }
  }

  void _back() {
    if (_step > 0) {
      setState(() => _step--);
    }
  }

  void _goTo(int step) => setState(() => _step = step);

  void _submit(BuildContext context) {
    final request = ProvisionTenantRequest(
      name: _name.text.trim(),
      slug: _slug.text.trim(),
      countryCode: _countryCode.text.trim().toUpperCase(),
      currencyCode: _currencyCode.text.trim().toUpperCase(),
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

  int _stepOfField(String field) => switch (field) {
    'Name' || 'Slug' || 'FirstBranchName' => 0,
    'CountryCode' || 'CurrencyCode' || 'TimeZoneId' => 1,
    _ => 2,
  };

  void _handleFailure(SignUpState state) {
    final target = state.errorCode == ApiErrorCodes.tenantSlugTaken
        ? 0
        : state.fieldErrors.keys
              .map(_stepOfField)
              .fold<int?>(null, (min, s) => min == null || s < min ? s : min);
    if (target != null && target != _step) {
      setState(() => _step = target);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return BlocConsumer<SignUpBloc, SignUpState>(
      listenWhen: (previous, current) =>
          current.status == SignUpStatus.failure &&
          previous.status != SignUpStatus.failure,
      listener: (context, state) => _handleFailure(state),
      builder: (context, state) {
        return Scaffold(
          appBar: AppBar(
            title: Text(l10n.appTitle),
            actions: const [ThemeMenuButton(), SizedBox(width: 4)],
          ),
          body: state.status == SignUpStatus.success && state.result != null
              ? _SuccessView(
                  businessName: _name.text.trim(),
                  tenantId: state.result!.tenantId,
                  handle: _slug.text.trim(),
                  onSignIn: widget.onSignIn,
                  onReset: () {
                    _resetForm();
                    context.read<SignUpBloc>().add(const SignUpReset());
                  },
                )
              : _responsiveBody(context, state, l10n),
        );
      },
    );
  }

  void _resetForm() {
    setState(() {
      _step = 0;
      _slugEdited = false;
      _name.clear();
      _slug.clear();
      _ownerName.clear();
      _ownerEmail.clear();
      _ownerPassword.clear();
    });
  }

  Widget _responsiveBody(
    BuildContext context,
    SignUpState state,
    AppLocalizations l10n,
  ) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final wide = constraints.maxWidth >= 900;
        if (!wide) {
          return _wizardPane(context, state, l10n);
        }
        return Row(
          children: [
            const Expanded(flex: 5, child: BrandPanel()),
            Expanded(flex: 6, child: _wizardPane(context, state, l10n)),
          ],
        );
      },
    );
  }

  /// The wizard, vertically centred when there is room and scrollable when there
  /// is not, so tall screens don't leave the form stranded at the top.
  Widget _wizardPane(
    BuildContext context,
    SignUpState state,
    AppLocalizations l10n,
  ) {
    return SafeArea(
      child: LayoutBuilder(
        builder: (context, constraints) {
          return SingleChildScrollView(
            child: ConstrainedBox(
              constraints: BoxConstraints(minHeight: constraints.maxHeight),
              child: Center(
                child: ConstrainedBox(
                  constraints: const BoxConstraints(maxWidth: 520),
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 24,
                      vertical: 32,
                    ),
                    child: _wizardColumn(context, state, l10n),
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _wizardColumn(
    BuildContext context,
    SignUpState state,
    AppLocalizations l10n,
  ) {
    final banner = _bannerError(l10n, state);
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        WizardProgress(
          currentStep: _step,
          labels: [
            l10n.stepBusinessLabel,
            l10n.stepRegionLabel,
            l10n.stepAccountLabel,
            l10n.stepReviewLabel,
          ],
        ),
        const SizedBox(height: 28),
        Text(
          l10n.stepOf(_step + 1, 4),
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
            color: Theme.of(context).colorScheme.primary,
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          _stepTitle(l10n),
          style: Theme.of(
            context,
          ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
        ),
        const SizedBox(height: 4),
        Text(
          _stepSubtitle(l10n),
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Theme.of(context).colorScheme.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: 24),
        if (banner != null) ...[
          InfoBanner(message: banner),
          const SizedBox(height: 16),
        ],
        _stepContent(context, state, l10n),
        const SizedBox(height: 28),
        _footer(context, state, l10n),
      ],
    );
  }

  String _stepTitle(AppLocalizations l10n) => switch (_step) {
    0 => l10n.businessStepTitle,
    1 => l10n.regionStepTitle,
    2 => l10n.accountStepTitle,
    _ => l10n.reviewStepTitle,
  };

  String _stepSubtitle(AppLocalizations l10n) => switch (_step) {
    0 => l10n.businessStepSubtitle,
    1 => l10n.regionStepSubtitle,
    2 => l10n.accountStepSubtitle,
    _ => l10n.reviewStepSubtitle,
  };

  Widget _stepContent(
    BuildContext context,
    SignUpState state,
    AppLocalizations l10n,
  ) {
    return switch (_step) {
      0 => _businessStep(l10n, state),
      1 => _regionStep(l10n, state),
      2 => _accountStep(l10n, state),
      _ => _reviewStep(l10n),
    };
  }

  Widget _businessStep(AppLocalizations l10n, SignUpState state) {
    return Form(
      key: _businessFormKey,
      child: Column(
        children: [
          _field(
            controller: _name,
            label: l10n.businessNameLabel,
            hint: l10n.businessNameHint,
            serverField: 'Name',
            state: state,
            prefixIcon: Icons.storefront_outlined,
            onChanged: (value) {
              if (!_slugEdited) {
                _slug.text = _slugify(value);
              }
            },
            validator: (v) =>
                _required(v) ? null : l10n.fieldRequired,
          ),
          _field(
            controller: _slug,
            label: l10n.slugLabel,
            helper: l10n.slugHelper,
            serverField: 'Slug',
            state: state,
            prefixIcon: Icons.link_outlined,
            onChanged: (_) => _slugEdited = true,
            validator: (v) {
              if (!_required(v)) return l10n.fieldRequired;
              final ok = RegExp(r'^[a-z0-9]+(-[a-z0-9]+)*$').hasMatch(v!.trim());
              return ok ? null : l10n.slugInvalid;
            },
          ),
          _field(
            controller: _branchName,
            label: l10n.firstBranchNameLabel,
            helper: l10n.firstBranchNameHelper,
            serverField: 'FirstBranchName',
            state: state,
            prefixIcon: Icons.store_outlined,
            validator: (v) => _required(v) ? null : l10n.fieldRequired,
          ),
        ],
      ),
    );
  }

  Widget _regionStep(AppLocalizations l10n, SignUpState state) {
    return Form(
      key: _regionFormKey,
      child: Column(
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: _field(
                  controller: _countryCode,
                  label: l10n.countryCodeLabel,
                  helper: l10n.countryCodeHelper,
                  serverField: 'CountryCode',
                  state: state,
                  validator: (v) =>
                      (v != null && v.trim().length == 2)
                      ? null
                      : l10n.fieldInvalidFormat,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _field(
                  controller: _currencyCode,
                  label: l10n.currencyCodeLabel,
                  helper: l10n.currencyCodeHelper,
                  serverField: 'CurrencyCode',
                  state: state,
                  validator: (v) =>
                      (v != null && v.trim().length == 3)
                      ? null
                      : l10n.fieldInvalidFormat,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String>(
            initialValue: _defaultLanguage,
            decoration: InputDecoration(
              labelText: l10n.defaultLanguageLabel,
              prefixIcon: const Icon(Icons.translate_outlined),
            ),
            items: [
              DropdownMenuItem(value: 'en', child: Text(l10n.languageEnglish)),
              DropdownMenuItem(value: 'rw', child: Text(l10n.languageKinyarwanda)),
              DropdownMenuItem(value: 'fr', child: Text(l10n.languageFrench)),
            ],
            onChanged: (value) =>
                setState(() => _defaultLanguage = value ?? 'en'),
          ),
          const SizedBox(height: 12),
          _field(
            controller: _timeZoneId,
            label: l10n.timeZoneLabel,
            serverField: 'TimeZoneId',
            state: state,
            prefixIcon: Icons.schedule_outlined,
            validator: (v) => _required(v) ? null : l10n.fieldRequired,
          ),
        ],
      ),
    );
  }

  Widget _accountStep(AppLocalizations l10n, SignUpState state) {
    return Form(
      key: _accountFormKey,
      child: Column(
        children: [
          _field(
            controller: _ownerName,
            label: l10n.ownerFullNameLabel,
            serverField: 'OwnerFullName',
            state: state,
            prefixIcon: Icons.person_outline,
            validator: (v) => _required(v) ? null : l10n.fieldRequired,
          ),
          _field(
            controller: _ownerEmail,
            label: l10n.ownerEmailLabel,
            serverField: 'OwnerEmail',
            state: state,
            prefixIcon: Icons.mail_outline,
            keyboardType: TextInputType.emailAddress,
            validator: (v) {
              if (!_required(v)) return l10n.fieldRequired;
              final ok = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$').hasMatch(v!.trim());
              return ok ? null : l10n.emailInvalid;
            },
          ),
          _field(
            controller: _ownerPassword,
            label: l10n.ownerPasswordLabel,
            helper: l10n.passwordHelper,
            serverField: 'OwnerPassword',
            state: state,
            prefixIcon: Icons.lock_outline,
            obscure: _obscurePassword,
            suffix: IconButton(
              icon: Icon(
                _obscurePassword
                    ? Icons.visibility_outlined
                    : Icons.visibility_off_outlined,
              ),
              tooltip: _obscurePassword ? l10n.showPassword : l10n.hidePassword,
              onPressed: () =>
                  setState(() => _obscurePassword = !_obscurePassword),
            ),
            validator: (v) =>
                (v != null && v.length >= 12) ? null : l10n.passwordTooShort,
          ),
        ],
      ),
    );
  }

  Widget _reviewStep(AppLocalizations l10n) {
    Widget section(String heading, int step, List<Widget> rows) {
      return Card(
        child: Padding(
          padding: const EdgeInsets.fromLTRB(16, 12, 8, 12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      heading,
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  TextButton.icon(
                    onPressed: () => _goTo(step),
                    icon: const Icon(Icons.edit_outlined, size: 16),
                    label: Text(l10n.editAction),
                  ),
                ],
              ),
              const SizedBox(height: 4),
              ...rows,
            ],
          ),
        ),
      );
    }

    return Column(
      children: [
        section(l10n.reviewBusinessHeading, 0, [
          ReviewRow(label: l10n.businessNameLabel, value: _name.text.trim()),
          ReviewRow(label: l10n.slugLabel, value: _slug.text.trim()),
          ReviewRow(
            label: l10n.firstBranchNameLabel,
            value: _branchName.text.trim(),
          ),
        ]),
        const SizedBox(height: 12),
        section(l10n.reviewRegionHeading, 1, [
          ReviewRow(
            label: l10n.countryCodeLabel,
            value: _countryCode.text.trim().toUpperCase(),
          ),
          ReviewRow(
            label: l10n.currencyCodeLabel,
            value: _currencyCode.text.trim().toUpperCase(),
          ),
          ReviewRow(
            label: l10n.defaultLanguageLabel,
            value: _languageName(l10n, _defaultLanguage),
          ),
          ReviewRow(label: l10n.timeZoneLabel, value: _timeZoneId.text.trim()),
        ]),
        const SizedBox(height: 12),
        section(l10n.reviewAccountHeading, 2, [
          ReviewRow(label: l10n.ownerFullNameLabel, value: _ownerName.text.trim()),
          ReviewRow(label: l10n.ownerEmailLabel, value: _ownerEmail.text.trim()),
          ReviewRow(
            label: l10n.ownerPasswordLabel,
            value: '•' * _ownerPassword.text.length,
          ),
        ]),
      ],
    );
  }

  Widget _footer(
    BuildContext context,
    SignUpState state,
    AppLocalizations l10n,
  ) {
    final isReview = _step == _reviewIndex;
    return Row(
      children: [
        if (_step > 0) ...[
          Expanded(
            child: OutlinedButton(
              onPressed: state.isSubmitting ? null : _back,
              child: Text(l10n.backButton),
            ),
          ),
          const SizedBox(width: 12),
        ],
        Expanded(
          flex: 2,
          child: FilledButton(
            onPressed: state.isSubmitting
                ? null
                : (isReview ? () => _submit(context) : _next),
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
                : Text(isReview ? l10n.createButton : l10n.continueButton),
          ),
        ),
      ],
    );
  }

  Widget _field({
    required TextEditingController controller,
    required String label,
    required String serverField,
    required SignUpState state,
    String? hint,
    String? helper,
    IconData? prefixIcon,
    Widget? suffix,
    bool obscure = false,
    TextInputType? keyboardType,
    ValueChanged<String>? onChanged,
    String? Function(String?)? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: TextFormField(
        controller: controller,
        obscureText: obscure,
        keyboardType: keyboardType,
        onChanged: onChanged,
        decoration: InputDecoration(
          labelText: label,
          hintText: hint,
          helperText: helper,
          helperMaxLines: 2,
          prefixIcon: prefixIcon == null ? null : Icon(prefixIcon),
          suffixIcon: suffix,
          errorText: _serverError(state, serverField),
        ),
        validator: validator,
      ),
    );
  }

  bool _required(String? value) => value != null && value.trim().isNotEmpty;

  String? _serverError(SignUpState state, String field) {
    final codes = state.fieldErrors[field];
    if (codes == null || codes.isEmpty) return null;
    final l10n = AppLocalizations.of(context);
    return switch (codes.first) {
      'REQUIRED' => l10n.fieldRequired,
      'INVALID_FORMAT' => l10n.fieldInvalidFormat,
      'TOO_SHORT' => l10n.fieldTooShort,
      'TOO_LONG' => l10n.fieldTooLong,
      _ => l10n.fieldInvalidFormat,
    };
  }

  String? _bannerError(AppLocalizations l10n, SignUpState state) {
    if (state.status != SignUpStatus.failure) return null;
    return switch (state.errorCode) {
      ApiErrorCodes.validationFailed => null,
      ApiErrorCodes.tenantSlugTaken => l10n.errorSlugTaken,
      ApiErrorCodes.network => l10n.errorNetwork,
      _ => state.errorMessage ?? l10n.errorUnexpected,
    };
  }

  String _languageName(AppLocalizations l10n, String code) => switch (code) {
    'rw' => l10n.languageKinyarwanda,
    'fr' => l10n.languageFrench,
    _ => l10n.languageEnglish,
  };
}

class _SuccessView extends StatelessWidget {
  const _SuccessView({
    required this.businessName,
    required this.tenantId,
    required this.handle,
    required this.onReset,
    this.onSignIn,
  });

  final String businessName;
  final String tenantId;
  final String handle;
  final VoidCallback onReset;
  final void Function(String handle)? onSignIn;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    return Center(
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 460),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                width: 88,
                height: 88,
                decoration: BoxDecoration(
                  color: scheme.primaryContainer,
                  shape: BoxShape.circle,
                ),
                child: Icon(
                  Icons.check_rounded,
                  size: 52,
                  color: scheme.onPrimaryContainer,
                ),
              ),
              const SizedBox(height: 24),
              Text(
                l10n.successTitle,
                textAlign: TextAlign.center,
                style: Theme.of(
                  context,
                ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
              ),
              const SizedBox(height: 8),
              Text(
                l10n.successBody(businessName),
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                  color: scheme.onSurfaceVariant,
                ),
              ),
              const SizedBox(height: 24),
              Text(
                l10n.successTenantHint,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: scheme.onSurfaceVariant,
                ),
              ),
              const SizedBox(height: 6),
              _TenantIdChip(tenantId: tenantId),
              const SizedBox(height: 32),
              if (onSignIn != null) ...[
                FilledButton(
                  onPressed: () => onSignIn!(handle),
                  child: Text(l10n.goToSignIn),
                ),
                const SizedBox(height: 12),
              ],
              OutlinedButton.icon(
                onPressed: onReset,
                icon: const Icon(Icons.add),
                label: Text(l10n.startOver),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _TenantIdChip extends StatelessWidget {
  const _TenantIdChip({required this.tenantId});

  final String tenantId;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final scheme = Theme.of(context).colorScheme;
    return Container(
      padding: const EdgeInsets.fromLTRB(16, 4, 4, 4),
      decoration: BoxDecoration(
        color: scheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Flexible(
            child: Text(
              tenantId,
              overflow: TextOverflow.ellipsis,
              style: const TextStyle(
                fontFeatures: [FontFeature.tabularFigures()],
              ),
            ),
          ),
          IconButton(
            icon: const Icon(Icons.copy_outlined, size: 18),
            tooltip: l10n.copyTooltip,
            onPressed: () async {
              await Clipboard.setData(ClipboardData(text: tenantId));
              if (context.mounted) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(content: Text(l10n.copiedMessage)),
                );
              }
            },
          ),
        ],
      ),
    );
  }
}
