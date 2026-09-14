# Firebot v2 — piano di lavoro

Riscrittura del mod, pensata per girare in tanti bot in parallelo (più CPU/RAM sensibile della v1).
Vive in `v2/` finché non è pronta a sostituire `src/`. Stesso nome/namespace/hotkey/cfg della v1
(`Firebot`, `firebot.dll`, `F7`, `FirebotPreferences.cfg`) — non serve conviverci fianco a fianco,
sostituirà la v1 quando è pronta. L'unica cautela: **la build non si copia da sola nella cartella
Mods del gioco** (`v2/src/Directory.Build.props` punta a `v2/dist/` locale) — altrimenti ogni build
di v2, finché è incompleta, sovrascriverebbe silenziosamente il firebot.dll v1 che sta girando
davvero. Quando è pronta a sostituirla per davvero, si copia `v2/dist/firebot.dll` in
`Firestone/Mods/` a mano (o lo faccio io su richiesta).

## Terminale di stato e file di configurazione (già presenti, portati dalla v1)

- **Tabella di stato task** (`Next Run` / `Time Left` / `Task` / `Status` / `Last Run`): stampata nel
  log dopo ogni esecuzione, identica alla v1 — è `BotManager.PrintTasksStatusTable()`, già portata.
  Compare per ogni `BotTask` (i task "commissione" dalla 2 in poi). `Hero Upgrade` non ci compare
  perché non è un task schedulato ma un'azione continua in background (gira sempre, non ha un
  "prossimo controllo" — come l'AutoUpgrade/AutoSkill della v1).
- **File di configurazione per abilitare/disabilitare i singoli task**: già presente, stesso
  meccanismo della v1 — `UserData/FirebotPreferences.cfg`, una sezione `[nome_task]` con `enabled =
  true/false` per ognuno, generata automaticamente al primo avvio (via MelonPreferences, non va
  scritta a mano).

## Cosa cambia rispetto alla v1 (e cosa NON cambia)

Non cambia: il modello a due livelli che già funziona in v1.
- **BotTask** (scheduler unico, un task alla volta, `NextRunTime` per il prossimo controllo) per le
  "commissioni" periodiche (Town, negozi, claim vari) — già efficiente, non è la fonte del problema.
- **GameElement / GameButton / GameText / CachedGameButton / GameNotificationButton** — i
  primitivi che risolvono i path e clickano. Portati quasi identici: non sono il collo di bottiglia.

Cambia (le due cose che davvero pesano su CPU/RAM con tanti bot):
1. **Upgrade eroi in battaglia**: in v1 gira come coroutine sempre attiva con hold+gap da 0.5s per
   bottone, in loop continuo — cicla forever anche quando non c'è nulla da fare. In v2: stesso
   modello (coroutine parallela, non nello scheduler principale, perché deve girare *durante* la
   battaglia indipendentemente dalle commissioni), ma: (a) i bottoni vengono risolti una volta e
   riusati tra i cicli invece di essere ricreati ogni giro, (b) il ritmo di polling passa da
   continuo a un intervallo configurabile (default 5s, come chiesto).
2. **Paths.cs**: in v1 è un unico file monolitico. In v2 diviso per schermata
   (`Infrastructure/Paths/Battle.cs`, `Paths/Town.cs`, ...), popolato in modo incrementale — solo
   i path che un task effettivamente usa, niente path "morti" copiati preventivamente. Fonte:
   `docs/index.html` + `docs/screens/*.html` (la mappa completa già fatta).

## Lista dei task (in ordine di implementazione — dal più semplice)

| # | Task | Stato in v1 | Note |
|---|------|-------------|------|
| 1 | **Hero Upgrade** (in battaglia) | Esiste (AutoUpgrade) | Porting + fix performance sopra. **Prossimo/fatto per primo.** |
| 2 | **Oracle's Gift** (claim giornaliero) | Path già noto, mai un task dedicato | Singolo bottone, notification-driven — primo task nuovo, prova del pattern BotTask "claim semplice". |
| 3 | **Daily Rewards + Value Bundle giornaliero** | DailyRewards esiste, i due bottoni (`grid/dailyRewardsButton`, `grid/valueBundleDailyButton`) sono nuovi | Fatto. Il claim nel tab "Pacchetti Giornalieri" è solo la mysteryBox gratuita (`freeText`) — gli slot numerati `valueBundle (0)/(1)/(2)` accanto sono acquisti veri, mai toccati. |
| 4 | **Quest giornaliere** (claim `quest (N)/claimedText`) | Non esiste | Da individuare schermata (probabile `Character` → tab Missioni, già mappata) e iterare gli slot come per le hero slot. |
| 5 | **Free Pickaxes** | Esiste | Porting diretto. |
| 6 | **Engineer** | Esiste | Porting diretto. |
| 7 | **Expeditions** | Esiste (rinominato meglio: v1 lo aveva in un file con nome sbagliato) | Porting diretto. |
| 8 | **Guardian Training** (Magic Quarters) | Esiste | Porting diretto. |
| 9 | **Oracle Rituals** | Esiste | Porting diretto. |
| 10 | **Experiments** (Alchemist) | Esiste | Porting, con opzione risorsa come in v1. |
| 11 | **Map Missions + Warfront Campaign Loot** | Esistono entrambi | Porting, due task correlati. |
| 12 | **Firestone Research** | Esiste | Il più complesso (logica priorità talenti) — ultimo. |

Eventuali aggiunte oltre questa lista (l'utente ha detto "forse anche qualcosa in più"): da valutare
una volta finita la lista sopra, usando le altre 36 schermate già mappate in `docs/screens/`.

## Come procediamo

Un task alla volta. Per ognuno: path nuovi in `Infrastructure/Paths/`, classe task, build, verifica
rapida che compili, poi passo al successivo solo dopo conferma.

## Stato attuale

- [x] Scheletro progetto (csproj, Directory.Build.props, Main.cs, core infra)
- [x] Task 1: Hero Upgrade
- [x] Task 2: Oracle's Gift
- [x] Task 3: Daily Rewards + Value Bundle
- [ ] Task 4: Quest giornaliere
- [ ] Task 5: Free Pickaxes
- [ ] Task 6: Engineer
- [ ] Task 7: Expeditions
- [ ] Task 8: Guardian Training
- [ ] Task 9: Oracle Rituals
- [ ] Task 10: Experiments
- [ ] Task 11: Map Missions + Warfront Campaign Loot
- [ ] Task 12: Firestone Research
