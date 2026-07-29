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
  String get brandTagline => 'Gérez toute votre entreprise au même endroit';

  @override
  String get brandBlurb =>
      'Ventes, stock, clients et rapports — réunis, dans votre langue. Configurons votre entreprise en quelques étapes simples.';

  @override
  String get brandPointCatalog => 'Suivez vos produits et votre stock';

  @override
  String get brandPointSales => 'Enregistrez ventes et paiements';

  @override
  String get brandPointInsights => 'Voyez comment se porte votre entreprise';

  @override
  String stepOf(int current, int total) {
    return 'Étape $current sur $total';
  }

  @override
  String get stepBusinessLabel => 'Entreprise';

  @override
  String get stepRegionLabel => 'Région';

  @override
  String get stepAccountLabel => 'Compte';

  @override
  String get stepReviewLabel => 'Vérification';

  @override
  String get businessStepTitle => 'Votre entreprise';

  @override
  String get businessStepSubtitle =>
      'Indiquez le nom de l\'entreprise que vous configurez.';

  @override
  String get regionStepTitle => 'Région et devise';

  @override
  String get regionStepSubtitle =>
      'Cela définit l\'affichage de la monnaie, des dates et de la langue.';

  @override
  String get accountStepTitle => 'Votre compte propriétaire';

  @override
  String get accountStepSubtitle =>
      'Vous l\'utiliserez pour vous connecter et tout gérer.';

  @override
  String get reviewStepTitle => 'Vérifier et créer';

  @override
  String get reviewStepSubtitle =>
      'Vérifiez les détails ci-dessous, puis créez votre entreprise.';

  @override
  String get businessNameLabel => 'Nom de l\'entreprise';

  @override
  String get businessNameHint => 'par ex. KwaConfi Depot';

  @override
  String get slugLabel => 'Identifiant de l\'entreprise';

  @override
  String get slugHelper =>
      'Lettres minuscules, chiffres et tirets. Utilisé dans votre adresse web.';

  @override
  String get firstBranchNameLabel => 'Nom de la première succursale';

  @override
  String get firstBranchNameHelper =>
      'Le lieu principal de vente. Vous pourrez en ajouter d\'autres.';

  @override
  String get countryCodeLabel => 'Code pays';

  @override
  String get countryCodeHelper => 'Deux lettres, par ex. RW';

  @override
  String get currencyCodeLabel => 'Code devise';

  @override
  String get currencyCodeHelper => 'Trois lettres, par ex. RWF';

  @override
  String get defaultLanguageLabel => 'Langue par défaut';

  @override
  String get timeZoneLabel => 'Fuseau horaire';

  @override
  String get ownerFullNameLabel => 'Votre nom complet';

  @override
  String get ownerEmailLabel => 'Votre e-mail';

  @override
  String get ownerPasswordLabel => 'Mot de passe';

  @override
  String get passwordHelper => 'Utilisez au moins 12 caractères.';

  @override
  String get showPassword => 'Afficher le mot de passe';

  @override
  String get hidePassword => 'Masquer le mot de passe';

  @override
  String get backButton => 'Retour';

  @override
  String get continueButton => 'Continuer';

  @override
  String get createButton => 'Créer l\'entreprise';

  @override
  String get creatingButton => 'Création…';

  @override
  String get editAction => 'Modifier';

  @override
  String get reviewBusinessHeading => 'Entreprise';

  @override
  String get reviewRegionHeading => 'Région';

  @override
  String get reviewAccountHeading => 'Compte propriétaire';

  @override
  String get successTitle => 'Votre entreprise est prête';

  @override
  String successBody(String name) {
    return '$name est maintenant configurée sur ConfiOS.';
  }

  @override
  String get successTenantHint => 'Conservez cet identifiant d\'entreprise :';

  @override
  String get startOver => 'Créer une autre entreprise';

  @override
  String get copyTooltip => 'Copier';

  @override
  String get copiedMessage => 'Copié dans le presse-papiers';

  @override
  String get fieldRequired => 'Ce champ est obligatoire';

  @override
  String get fieldInvalidFormat => 'Cette valeur n\'a pas le format attendu';

  @override
  String get fieldTooShort => 'Cette valeur est trop courte';

  @override
  String get fieldTooLong => 'Cette valeur est trop longue';

  @override
  String get emailInvalid => 'Saisissez une adresse e-mail valide';

  @override
  String get slugInvalid =>
      'Utilisez uniquement lettres minuscules, chiffres et tirets';

  @override
  String get passwordTooShort => 'Utilisez au moins 12 caractères';

  @override
  String get errorSlugTaken =>
      'Cet identifiant d\'entreprise est déjà utilisé. Essayez-en un autre.';

  @override
  String get errorNetwork =>
      'Impossible de joindre le serveur. Vérifiez votre connexion et réessayez.';

  @override
  String get errorUnexpected =>
      'Une erreur s\'est produite. Veuillez réessayer.';

  @override
  String get signInTitle => 'Bon retour';

  @override
  String get signInSubtitle => 'Connectez-vous à votre entreprise.';

  @override
  String get handleFieldLabel => 'Identifiant de l\'entreprise';

  @override
  String get signInHandleHelper =>
      'L\'identifiant que vous avez choisi en créant l\'entreprise.';

  @override
  String get emailFieldLabel => 'E-mail';

  @override
  String get passwordFieldLabel => 'Mot de passe';

  @override
  String get signInButton => 'Se connecter';

  @override
  String get signingInButton => 'Connexion…';

  @override
  String get noBusinessPrompt => 'Nouveau sur ConfiOS ?';

  @override
  String get createBusinessLink => 'Créer une entreprise';

  @override
  String get haveBusinessPrompt => 'Vous avez déjà une entreprise ?';

  @override
  String get signInLink => 'Se connecter';

  @override
  String get goToSignIn => 'Se connecter à votre entreprise';

  @override
  String homeWelcome(String name) {
    return 'Bienvenue, $name';
  }

  @override
  String homeSignedInTo(String business) {
    return 'Vous êtes connecté à $business.';
  }

  @override
  String get homeBusinessIdLabel => 'Identifiant d\'entreprise';

  @override
  String get homePermissionsLabel => 'Autorisations';

  @override
  String homePermissionsCount(int count) {
    return '$count accordées';
  }

  @override
  String get homeComingSoon =>
      'Votre tableau de bord s\'enrichira ici à mesure que le Catalogue, l\'Inventaire et les Ventes seront développés.';

  @override
  String get signOut => 'Se déconnecter';

  @override
  String get productsTitle => 'Produits';

  @override
  String get openCatalog => 'Ouvrir le catalogue';

  @override
  String get addProduct => 'Ajouter un produit';

  @override
  String get productNameLabel => 'Nom du produit';

  @override
  String get productSkuLabel => 'SKU';

  @override
  String get productPriceLabel => 'Prix';

  @override
  String get createProductButton => 'Créer le produit';

  @override
  String get noProductsTitle => 'Aucun produit pour le moment';

  @override
  String get noProductsHint => 'Ajoutez votre premier produit pour commencer.';

  @override
  String get productsLoadError => 'Impossible de charger les produits.';

  @override
  String get retryButton => 'Réessayer';

  @override
  String get themeTooltip => 'Thème';

  @override
  String get themeSystem => 'Système';

  @override
  String get themeLight => 'Clair';

  @override
  String get themeDark => 'Sombre';

  @override
  String get languageEnglish => 'Anglais';

  @override
  String get languageKinyarwanda => 'Kinyarwanda';

  @override
  String get languageFrench => 'Français';
}
