# Task Board per Coach Sportivo

Applicazione web full-stack progettata per aiutare i coach sportivi a gestire le attività, le scadenze e il flusso di lavoro legato ai loro atleti. Questo progetto è stato sviluppato come contributo originale per la mia tesi di laurea triennale in Informatica, generalizzando i concetti di scheduling appresi durante un'esperienza di stage in un contesto manifatturiero.


##  Scopo del Progetto

Lo strumento nasce dall'esigenza di fornire ai coach un cruscotto visivo e interattivo per monitorare le attività imminenti, come la preparazione di nuove schede di allenamento, il controllo dei video tecnici degli atleti o la gestione delle scadenze degli abbonamenti. L'obiettivo è trasformare una lista di compiti in un flusso di lavoro organizzato, migliorando l'efficienza e riducendo il rischio di dimenticanze.

---



##  Come Eseguire il Progetto in Locale
1.  **Prerequisiti:**
    *   .NET SDK (versione X.X)
    *   SQL Server (Express o Developer Edition)
    *   Visual Studio 2022 o un editor di codice come VS Code.

2.  **Configurazione:**
    *   Clonare la repository:
    *   Creare un file `appsettings.Development.json` nella root del progetto.
    *   Aggiungere la stringa di connessione al proprio database SQL Server locale nel file appena creato.
    *   Eseguire le migrazioni di Entity Framework per creare il database: `dotnet ef database update`.

3.  **Avvio:**
    *   Avviare l'applicazione da Visual Studio o tramite il comando `dotnet run`.

---
