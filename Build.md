# Guide de Build et Installation iOS

Ce guide explique comment builder MotoGPS pour iOS et l'installer sur ton iPhone via Sideloadly.

## Prérequis

- **Sideloadly** installé sur ton Windows 11 → [Télécharge ici](https://sideloadly.io/)
- Un **câble USB** pour connecter ton iPhone
- Un **compte Apple ID gratuit** (pas besoin de payer 99$/an)
- Ton iPhone doit avoir un **code de déverrouillage** configuré (Face ID, Touch ID, ou code PIN)

## Étapes

### 1. Builder l'IPA depuis GitHub Actions

1. Va sur le dépôt GitHub : https://github.com/oscar_clv/MotoGPS
2. Clique sur l'onglet **Actions**
3. Sélectionne le workflow **iOS Build for Sideload** (à gauche)
4. Clique sur **Run workflow** (bouton gris/blanc)
5. Clique à nouveau sur **Run workflow** pour confirmer

Le build va prendre **~3-5 minutes**. Tu verras une barre de progression.

### 2. Télécharger l'IPA

1. Une fois le build terminé (✅ vert), clique sur le nom du workflow qui vient de finir
2. Scroll en bas jusqu'à la section **Artifacts**
3. Clique sur `MotoGPS-iOS` pour télécharger le fichier `.ipa`
4. Enregistre le fichier sur ton Bureau (ou n'importe où sur ton PC)

### 3. Installer l'IPA avec Sideloadly

1. **Lance Sideloadly** sur ton Windows 11
2. **Branche ton iPhone en USB** à ton PC
3. Entre ton **Apple ID** et ton **mot de passe** dans Sideloadly
   - ⚠️ Apple demande une vérification 2FA — tu vas recevoir une notification sur l'iPhone
   - Approuve la connexion sur ton iPhone
4. Glisse le fichier `.ipa` dans la fenêtre de Sideloadly (ou clique sur le bouton "Select IPA")
5. Clique sur **Install**
6. Sideloadly va re-signer l'app avec ton Apple ID et l'installer sur l'iPhone
7. Attends que ça dise "Installation Complete ✅"

L'app est maintenant sur ton iPhone ! 🎉

### 4. Lancer l'app

Va dans l'écran d'accueil de ton iPhone → trouve **MotoGPS** → appuie pour lancer.

## Important : Durée de vie de l'app

Avec un **Apple ID gratuit**, l'application a une **durée de vie de 7 jours**. Après 7 jours, elle va se fermer automatiquement.

**Pour la réinstaller :**
1. Fais un nouveau build dans GitHub Actions (Étape 1)
2. Télécharge le nouvel IPA
3. Relance Sideloadly avec le nouvel IPA

C'est le prix du gratuit — tu n'as besoin de rien payer, juste de re-signer une fois par semaine.

## Dépannage

### L'app s'installe mais elle n'apparaît pas sur l'iPhone
- Vérifie que tu vois bien "Installation Complete ✅" dans Sideloadly
- Redémarre ton iPhone
- Essaie de relancer Sideloadly

### Sideloadly dit "Access Denied" ou "Failed to install"
- Vérifie que tu as approuvé la demande d'appairage sur l'iPhone
- Vérifiez que tu as un code de déverrouillage sur l'iPhone (obligatoire)
- Redémarre Sideloadly

### Le build échoue sur GitHub
- Vérifie que tu as pushé les derniers changements
- Essaie de relancer le workflow (bouton "Run workflow")
- Si ça persiste, envoie le log d'erreur

## Aller plus loin

Si tu veux modifier l'app :
1. Fais tes changements sur ton PC
2. Push vers GitHub
3. Le workflow GitHub Actions rebuild automatiquement à chaque push
4. Réinstalle via Sideloadly chaque semaine

Enjoy ! 🚀
