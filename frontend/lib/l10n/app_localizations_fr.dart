// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for French (`fr`).
class AppLocalizationsFr extends AppLocalizations {
  AppLocalizationsFr([String locale = 'fr']) : super(locale);

  @override
  String get appTitle => 'ConfiOS';

  @override
  String get signUpTitle => 'Créez votre entreprise';

  @override
  String get signUpSubtitle =>
      'Configurez votre entreprise sur ConfiOS en une minute.';

  @override
  String get businessNameLabel => 'Nom de l\'entreprise';

  @override
  String get slugLabel => 'Identifiant de l\'entreprise';

  @override
  String get slugHelper =>
      'Lettres minuscules, chiffres et tirets, par ex. kwaconfi-depot';

  @override
  String get countryCodeLabel => 'Code pays';

  @override
  String get currencyCodeLabel => 'Code devise';

  @override
  String get defaultLanguageLabel => 'Langue par défaut';

  @override
  String get timeZoneLabel => 'Fuseau horaire';

  @override
  String get firstBranchNameLabel => 'Nom de la première succursale';

  @override
  String get ownerFullNameLabel => 'Votre nom complet';

  @override
  String get ownerEmailLabel => 'Votre e-mail';

  @override
  String get ownerPasswordLabel => 'Mot de passe';

  @override
  String get createButton => 'Créer l\'entreprise';

  @override
  String get creatingButton => 'Création…';

  @override
  String get successTitle => 'Entreprise créée';

  @override
  String successBody(String tenantId) {
    return 'Votre entreprise est prête. Le locataire $tenantId est maintenant actif.';
  }

  @override
  String get startOver => 'En créer une autre';

  @override
  String get fieldRequired => 'Ce champ est obligatoire';

  @override
  String get fieldInvalidFormat => 'Cette valeur n\'a pas le format attendu';

  @override
  String get fieldTooShort => 'Cette valeur est trop courte';

  @override
  String get fieldTooLong => 'Cette valeur est trop longue';

  @override
  String get errorSlugTaken =>
      'Cet identifiant d\'entreprise est déjà utilisé.';

  @override
  String get errorNetwork =>
      'Impossible de joindre le serveur. Vérifiez votre connexion et réessayez.';

  @override
  String get errorUnexpected =>
      'Une erreur s\'est produite. Veuillez réessayer.';

  @override
  String get languageEnglish => 'Anglais';

  @override
  String get languageKinyarwanda => 'Kinyarwanda';

  @override
  String get languageFrench => 'Français';
}
