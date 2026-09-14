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
| 4 | **Quest giornaliere** (claim `quest (N)/claimedText`) | Non esiste | Fatto. Screen `Character` → tab Missioni → sotto-tab Giornaliere, 9 slot fissi (`quest (0)`-`(8)`), click su tutti i `claimButton` (no-op sicuro su quelli non completi). Root path di `Character` non verificato dal vivo (nessun corrispettivo v1) — inferito per coerenza con Store/OracleStore. |
| 5 | **Free Pickaxes** | Esiste | Fatto. A differenza della v1, aggiunta anche la navigazione manuale esplicita (rightSideUI/guildButton → TownGuild/guildShop → GuildShop/supplies), non presente né in v1 né nella prima porting: prima c'era solo il path via notifica. Nessun NotificationPath, di proposito: il claim è a soglia (`pickaxe_claim_threshold`), quindi il badge attivo non garantisce che ci sia abbastanza da riscattare — stessa scelta della v1. |
| 6 | **Engineer** | Esiste | Fatto. Porting + stessa aggiunta del Task 5: navigazione manuale esplicita (rightSideUI/townButton → TownIrongard/townBg/parent/engineer), non presente in v1 (solo notifica). NotificationPath presente: claim singolo a cooldown fisso, badge affidabile (a differenza di Free Pickaxes che è a soglia). |
| 7 | **Expeditions** | Esiste (rinominato meglio: v1 lo aveva in un file con nome sbagliato) | Fatto. Porting + stessa aggiunta dei task precedenti: navigazione manuale esplicita (rightSideUI/guildButton → TownGuild/expeditions), non presente in v1. NotificationPath mantenuto dalla v1 nonostante il claim+start combinati (comportamento già live-testato, non modificato). |
| 8 | **Guardian Training** (Magic Quarters) | Esiste | Fatto. Porting + navigazione manuale esplicita (rightSideUI/townButton → TownIrongard/townBg/parent/magicQuarters), non presente in v1. Logica di selezione guardiano e "strange dust" invariata dalla v1. |
| 9 | **Oracle Rituals** | Esiste | Fatto. Porting + navigazione manuale esplicita (rightSideUI/townButton → TownIrongard/townBg/parent/oracle), non presente in v1. Schermata "Oracle" (rituali) distinta da "OracleStore" (Task 2, negozio regali). |
| 10 | **Experiments** (Alchemist) | Esiste | Fatto. Porting + navigazione manuale esplicita (rightSideUI/townButton → TownIrongard/townBg/parent/alchemist). Comportamento di default invariato dalla v1: se `resource_type` non è configurato, non claima/avvia nulla (sono risorse limitate reali - Dragon blood/Strange dust/Exotic coin - richiede opt-in esplicito, stesso principio del Task 3 sugli acquisti). Aggiunto `BotSettings.FreeSpeedupSeconds` (mancava in v2, usato anche dai task futuri 11/12). |
| 11 | **Map Missions + Warfront Campaign Loot** | Esistono entrambi | Fatto. Portati come 2 task separati (stessa granularità della v1: MapMissions è badge-driven con `NotificationPath`, WarfrontCampaignLoot è a solo cooldown, girano su schedule indipendenti quindi unirli non avrebbe risparmiato click). Aggiunta navigazione manuale esplicita comune (rightSideUI/mapButton → WorldMap, poi il tab giusto: submenuButtons/mapMissionsButton o /warfrontCampaignButton), non presente in v1. Path dei pin missione (`menusRoot/mapRoot/mapElements/missions`) confermati dalla v1: vivono fuori da menuCanvas, sulla mappa di sfondo sempre visibile, non dentro il tab. |
| 12 | **Firestone Research** | Esiste | Fatto, poi rivisto su richiesta dell'utente. Navigazione manuale esplicita (rightSideUI/townButton → TownIrongard/townBg/parent/library → submenuButtons/firestoneResearch), non presente in v1. Nota: la schermata Library ha 2 tab (meteoriteResearch/firestoneResearch) e quello giusto NON è selezionato di default all'apertura (docs/screens/Library.html) — mancava nel primo giro, trovato da un audit e corretto (click sul tab aggiunto). Path del tab non verificato dal vivo (nessun corrispettivo v1, solo fonte doc). **Priorità talenti rivista** (non più "livello più basso, poi tempo minore" come la v1): ora sceglie sempre e solo il talento con tempo di completamento minore tra tutti quelli sbloccati, su tutti e 3 gli alberi (prima ne scansionava solo uno, quello lasciato selezionato) — evita di portare un singolo talento a lvl 20 in 10 giorni quando nello stesso tempo si potrebbe alzare l'intera alberatura, stesso principio dietro anche il Task 14. |
| 14 | **Meteorite Research** (secondo tab della Library, aggiunto dopo la lista iniziale) | Non esiste, mai implementata (badge notifica presente ma "Rimossa dal bot, feature mai raggiunta") | Fatto. Nessun corrispettivo v1 — path trovati con una nuova scansione UnityPy diretta sugli asset di gioco (non solo dai doc, che non avevano catturato il popup di anteprima né il costo per nodo). 5 alberi × 13 nodi, click su un nodo apre sempre `MeteoriteResearchPreview` (mai un upgrade diretto) che mostra costo/stato sblocco — a differenza della ricerca standard qui non c'è modo noto di leggere il saldo delle pietre di meteorite senza aprire la Library (nessuna barra valute persistente trovata nemmeno sulla schermata di battaglia, verificato: `topRightSideUINew`, la variante live, non ha alcun contatore valuta). Di conseguenza il task scansiona sempre tutti e 5 gli alberi cercando il nodo sbloccato più economico e tenta il click (no-op sicuro se non ancora permesso), poi riprova dopo `recheck_interval_minutes` (default 60) invece di controllare di continuo. Nessun NotificationPath (soglia di valuta, stesso motivo di Free Pickaxes/Empower); notifica opportunistica aggiunta ma non verificata dal vivo (nessun precedente v1). |
| 13 | **Empower** (Temple of Eternals, aggiunto dopo la lista iniziale) | Esiste (`TempleOfEternalsTask`) | Fatto. Non è un boost temporaneo: è il reset/prestige del Tempio degli Eterni (banca le Firestone trovate nell'avventura corrente, alza il moltiplicatore permanente, riparte l'avventura). Gate a scelta dell'utente confermata: stessa logica v1 (rapporto Firestone trovate/possedute, default 2.0x, `min_reset_ratio`) + minuti minimi/massimi di avventura (`min_adventure_minutes`/`max_adventure_minutes`, default 60/120) — non un check su "monete e gemme guadagnate" perché il pannello non espone nessuna statistica "gemme guadagnate" (le gemme sono la valuta premium usata altrove per acquisti reali) né un rapporto per l'oro (solo un totale assoluto). Navigazione manuale esplicita aggiunta (rightSideUI/townButton → TownIrongard/townBg/parent/templeOfEternals). Notifica aggiunta (`TemplePrestige`) ma **non verificata dal vivo**: a differenza di ogni altro task, la v1 non aveva mai usato una notifica per questa feature, il badge esiste solo nella scansione statica dei doc — deliberatamente NON usata per lo scheduling (`NotificationPath`), solo come fast-path opportunistico in `Execute()`, stessa scelta di Free Pickaxes visto che è comunque un task a soglia. `AutoRetreat.OnAdventureReset()` della v1 non è stato portato (AutoRetreat è una feature separata, mai richiesta, non esiste in v2) — punto di aggancio lasciato commentato nel codice se in futuro si vorrà portare anche quella. |

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
- [x] Task 4: Quest giornaliere
- [x] Task 5: Free Pickaxes
- [x] Task 6: Engineer
- [x] Task 7: Expeditions
- [x] Task 8: Guardian Training
- [x] Task 9: Oracle Rituals
- [x] Task 10: Experiments
- [x] Task 11: Map Missions + Warfront Campaign Loot
- [x] Task 12: Firestone Research
- [x] Task 13: Empower (Temple of Eternals)
- [x] Task 14: Meteorite Research
