# Piano di test dal vivo

Nessun task è mai stato testato dentro il gioco vero - solo verificato staticamente (scansioni
UnityPy + build pulita). Questo documento guida il primo giro di test reali, un task alla volta.

**Aggiornamento dopo il primo giro (vedi `PLAN.md`, sezione "Primo giro di test dal vivo")**: trovati
e corretti diversi bug reali di navigazione (Free Pickaxes, Oracle's Gift, Character/Quests, rail di
notifica, Oracle Rituals/Experiments/Firestone Research, animazione Awakening).

**Aggiornamento dopo il secondo giro (2026-09-17)**: confermato dal vivo che `TownIrongard` era
davvero `menus/TownIrongard`, non `popups/TownIrongard` come cambiato nel giro precedente - riportato
indietro (vedi `PLAN.md`). Da lì passano moltissimi task, quindi questo sblocca la maggior parte
della lista sotto. Trovati e corretti anche due bug propri di Firestone Research (scansionava tier
bloccati invece di fermarsi al primo non sbloccato; crashava silenziosamente se restava un secondo
slot vuoto da riempire dopo aver avviato la prima ricerca) e applicato preventivamente lo stesso fix
a Meteorite Research, che ha la stessa identica struttura. Testati dal vivo e confermati funzionanti:
Guardian Training, Daily Store Offers, Firestone Research, Meteorite Research (vedi righe sotto).
**Punto aperto, non bloccante**: dopo Firestone Research il bot a volte sembra restare sulla
schermata città invece di tornare alla battaglia come gli altri task - non ancora isolato con
certezza (potrebbe essere un altro task, es. Daily Store Offers, che riapre Town subito dopo).
Da riverificare in un giro dedicato, ignorato per ora.

**Aggiornamento (2026-09-17, System Mail)**: scoperta importante confermata dall'utente - il gioco
sceglie tra **due varianti HUD parallele in base alla risoluzione/aspect ratio del client**, non è
un'ambiguità di versione. Un path hardcoded su una sola variante funziona in metà delle sessioni e
fallisce silenziosamente nell'altra metà. Corretto sistematicamente con un helper
(`UiVariantButton`, prova più path candidati) applicato al bottone mail e a **tutta** la rail di
notifica (`NotificationsLoc`, usata da quasi ogni task come fast-path opzionale + segnale di
priorità) - vedi `PLAN.md`. **Non ancora esteso** all'ambiguità Desktop/Mobile/New della barra
inferiore (Path of Glory/Inventory/Party/Hero Upgrade) - nessun test dal vivo l'ha ancora toccata.
Testato dal vivo e confermato funzionante: Mailbox (System Mail) - claim riuscito, popup "Rewards"
di conferma chiuso correttamente dal Watchdog generico (non serve un click esplicito su "OK").

**Aggiornamento (2026-09-18, Collector/apertura chest)**: causa radice trovata e risolta. Gli slot
chest in Inventario (`Common`/`Uncommon`/`Rare`/`Epic`, celle riciclate della ScrollView) hanno un
componente `Button` presente/abilitato ma con **zero listener su `onClick`** - il vero click nel
gioco reagisce direttamente agli eventi puntatore, non a `Button.onClick`. `GameButton.Click()`
(che chiama `onClick.Invoke()`) per questo non faceva nulla, mentre un click reale dell'utente
funzionava. Aggiunto `GameButton.ClickSimulated()`, che rigioca la sequenza
PointerDown/Up/Click tramite `UnityEngine.EventSystems.ExecuteEvents` direttamente sul
GameObject target - è una chiamata puramente in-process nell'EventSystem di Unity di
quell'istanza, non tocca il mouse/cursore reale del sistema operativo, quindi resta sicura su ~20
istanze del gioco in parallelo. Confermato dal vivo: la preview si apre e le chest si aprono
(Rare/Epic svuotate, Uncommon ridotta). Aggiunto anche un poll (non un timer fisso) che aspetta il
prossimo bottone (`openx1`/`openx10`) diventi cliccabile prima del click successivo, perché
l'animazione di apertura ha durata variabile e un delay fisso usciva troppo presto rischiando di
saltare l'apertura successiva. **Punto ancora aperto**: il presunto "secondo schermo" `ChestOpening`
(per incatenare più aperture dello stesso tipo senza tornare alla preview) non è mai stato
osservato esistere nella gerarchia finché non serve più di un click - probabile che con quantità
piccole tutto avvenga in un solo click sulla preview stessa; il codice lo gestisce comunque come
no-op sicuro se quello schermo non esiste. Resta da verificare con una chest che richieda più di un
ciclo (es. scorta comune grande) se quel percorso serve davvero o va rimosso.

Per velocizzare i cicli di test in questa sessione, `auto_start` è stato temporaneamente messo a
`true` (con `start_bot_delay` al minimo consentito di 10s) invece di `false` come raccomandato
sotto - il bot parte da solo ad ogni avvio del gioco senza bisogno di premere F7. Ricordarsi di
rimetterlo a `false` a fine sessione di test se si torna alla procedura "un task alla volta" con
osservazione manuale.

## Perché un task alla volta

Attivare tutto insieme renderebbe impossibile capire quale azione ha causato quale effetto nel
gioco. La procedura è: abilita **un solo** task nel file di configurazione, avvia il gioco, osserva
se fa quello che dovrebbe, poi disabilitalo prima di passare al successivo.

`auto_start` deve restare `false` per tutta la fase di test: si avvia il bot a mano con `F7` solo
quando si vuole osservare quel task specifico, poi si preme di nuovo `F7` per fermarlo prima di
richiudere il gioco e modificare il file.

## Prima di iniziare

1. **Chiudi completamente il gioco** (il file `firebot.dll` è bloccato mentre Firestone è aperto).
2. Il file di configurazione reale è in `Firestone/UserData/FirebotPreferences.cfg` (diverso dal
   vecchio `config/FirebotPreferences.cfg` che era nel repository - quello era solo un riferimento
   statico, mai caricato dal gioco, ed è stato rimosso nella pulizia).
3. **I nomi delle sezioni sono cambiati** rispetto a prima: ora corrispondono al nome della classe
   C# del task (es. `[alchemist]` è diventato `[experimentstask]`, `[oracle]` è diventato
   `[oracleritualstask]`). Le vecchie sezioni con `enabled = true` restano nel file ma non
   corrispondono più a nessun task reale - il bot le ignora, non c'è rischio che qualcosa parta da
   solo al primo avvio. Ogni sezione elencata sotto va cercata/creata con il nome esatto indicato.
4. Verifica che `debug_mode = true` sotto `[firebot_settings]`, per avere log dettagliati in
   console durante i test.
5. Log da tenere d'occhio: `Firestone/MelonLoader/Latest.log` - ogni task stampa quando parte,
   cosa trova, e quando pianifica il prossimo controllo.

## Procedura per ogni task

1. Chiudi il gioco (se aperto).
2. Apri `FirebotPreferences.cfg`, trova la sezione del task (o creala se non esiste ancora - basta
   avviare il bot una volta con tutto spento perché tutte le sezioni vengano generate), imposta
   `enabled = true` sotto quella sezione soltanto.
3. Salva, avvia il gioco, aspetta il caricamento completo, premi `F7`.
4. Osserva il comportamento atteso (vedi la tabella sotto) e il log per eventuali errori.
5. Premi di nuovo `F7` per fermare il bot, chiudi il gioco.
6. Rimetti `enabled = false` per quel task prima di passare al successivo.

Alcuni task richiedono uno stato specifico nel gioco per avere qualcosa da fare (es. missioni già
completate, valuta accumulata) - se non hanno nulla da fare la prima volta, è normale: riprova dopo
aver raggiunto quella condizione, o confronta comunque il comportamento "niente da fare" con quanto
descritto (dovrebbe restare inerte senza errori, non bloccarsi).

---

## Comportamenti sempre attivi (non sono nello scheduler dei task)

Questi due non hanno una sezione "un task alla volta" nello stesso senso - partono e si fermano con
`F7` insieme al resto, ma girano in continuo invece che a schedulazione.

| # | Nome | Sezione cfg | Comportamento atteso |
|---|------|--------------|----------------------|
| - | **Hero Upgrade** | `[hero_upgrade]` | Durante una battaglia, il leader e ogni slot eroe vengono potenziati automaticamente (bottone upgrade tenuto premuto a intervalli). Verifica che il gold cali e i livelli salgano, senza click visibili su eroi non ancora sbloccati. |
| - | **AutoRetreat** | `[auto_retreat]` | Se lo stage in battaglia non avanza per `stall_minutes` (default 3 min), il bot dovrebbe cliccare la freccia "torna indietro" nello stage in battaglia `retreat_stages` volte (default 5). Per testarlo in tempi ragionevoli, abbassa temporaneamente `stall_minutes` a 1 e fermati apposta su uno stage duro. |

---

## Quests (6 task che gestiscono le missioni giornaliere)

| # | Nome | Sezione cfg | Livello min. | Comportamento atteso |
|---|------|--------------|:---:|----------------------|
| 1 | ✅ Quests (claim giornaliere/settimanali) | `[queststask]` | - | Apre Character → Missioni, clicca claim su ogni missione già completata (giornaliere e settimanali), lascia stare quelle non ancora fatte. **Testato 2026-09-17: funziona, switcha correttamente tra daily e weekly.** |
| 2 | ⚠️ Collector | `[collectorquesttask]` | - | Apre l'Inventario, apre le chest (gear/jewel/celestial) tenendo da parte `min_common_chest_reserve` (default 10) chest comuni - il numero di chest comuni in inventario non dovrebbe scendere sotto quella soglia. **Testato 2026-09-18: risolto il bug di fondo (click sullo slot chest non arrivava a destinazione, vedi nota sopra) - confermato dal vivo che le chest si aprono davvero (Rare/Epic svuotate, Uncommon ridotta). Funziona ma non è ancora perfetto: i tempi tra un'apertura e l'altra vanno ottimizzati ulteriormente (il poll attuale aiuta ma non è la soluzione definitiva). Percorso "apri più lotti di fila senza richiudere" non ancora esercitato da un caso reale. Da rivedere in un giro dedicato.** |
| 3 | Gamer | `[gamerquesttask]` | 15 | In Taverna, gioca fino a 10 partite con i Game Token, lasciandone almeno `min_token_reserve` di scorta. |
| 4 | BeerExchange | `[beerexchangetask]` | 15 | Taverna → Mercato: compra ripetutamente il pacchetto da 5 token finché conviene. **Punto da verificare con attenzione**: deve spendere birra, non gemme - controlla il saldo gemme prima/dopo, se scende qualcosa non va. |
| 5 | Merchant | `[merchantquesttask]` | 30 | Exotic Merchant: usa tutti gli oggetti oro in inventario, vende in x1 ogni oggetto rimasto in griglia, fa un solo upgrade (il primo disponibile). |
| 6 | Miner | `[minerquesttask]` | 50 | Gilda → Cristallo Arcano: colpisce il cristallo 5 volte (click singoli). |

## Town (edifici cittadini)

| # | Nome | Sezione cfg | Livello min. | Comportamento atteso |
|---|------|--------------|:---:|----------------------|
| 7 | ✅ Daily Store Offers | `[dailystoreofferstask]` | - | Reclama la ricompensa giornaliera di accesso e la mystery box gratuita giornaliera; non tocca i bundle a pagamento accanto. **Testato 2026-09-17: mystery box confermata, funziona.** |
| 8 | ✅ Engineer | `[engineertask]` | 50 | Reclama gli strumenti pronti dall'Ingegnere quando disponibili. **Testato 2026-09-17: funziona, letto timer reale (6h) dal quick-access della notifica.** |
| 9 | War Machines | `[warmachinestask]` | 50 | Town → Engineer → War Machines → tab Workshop: livella ogni war machine posseduta finché il bottone di livellamento resta cliccabile (richiede Expedition Token + componenti). |
| 10 | ✅ Guardian Training | `[guardiantrainingtask]` | - | Magic Quarters: avvia l'allenamento sul guardiano configurato (`guardian_index`). **Testato 2026-09-17: funziona, torna correttamente alla schermata di battaglia.** |
| 11 | Experiments (Alchemist) | `[experimentstask]` | 120 | Avvia/reclama esperimenti in Alchemist; se `resource_type` è vuoto non fa nulla (comportamento voluto, di norma da configurare esplicitamente). |
| 12 | Oracle Rituals | `[oracleritualstask]` | 200 | Reclama rituali completati e ne avvia uno nuovo. |
| 13 | Oracle's Gift | `[oraclesgifttask]` | 200 | Reclama il regalo giornaliero dell'Oracolo. |
| 14 | ✅ Firestone Research | `[firestoneresearchtask]` | - | Library → tab Firestone Research: avvia/reclama ricerca, con "Raining Gold" sempre priorità se disponibile. **Testato 2026-09-17 (3 giri, 2 bug trovati e corretti - vedi nota in alto): ora riempie correttamente più slot vuoti in un solo run, senza scansionare tier bloccati.** |
| 15 | ✅ Meteorite Research | `[meteoriteresearchtask]` | - | Library → tab Meteorite Research: stesso principio, sui 5 alberi di meteorite. **Testato 2026-09-17 (con lo stesso fix applicato preventivamente): funziona.** |
| 16 | Temple of Eternals (Empower) | `[empowertask]` | - | Fa il reset/prestige solo quando il rapporto Firestone trovate/possedute e i minuti di avventura configurati sono soddisfatti - non dovrebbe mai fare empower "a caso". |
| 17 | Free Pickaxes | `[freepickaxestask]` | 50 | Reclama piccozze gratuite solo una volta raggiunta la soglia `pickaxe_claim_threshold`. |
| 18 | Scarab's Game (omaggio) | `[scarabgamefreetokentask]` | 60 | Taverna → Scarab's Game → shop: reclama l'omaggio giornaliero gratuito nel tab Saldi. |
| 19 | Pharaoh's Vault + spin | `[pharaohsvaulttask]` | 60 | Gira la slot con i Noble Token gratuiti, apre il Pharaoh's Vault quando ci sono abbastanza Ancient Coin. |
| 20 | ✅ Mailbox | `[systemmailtask]` | - | Reclama ogni ricompensa in posta (Arcane Crystal, traguardi livello, rank Arena, Battle Pass) - non deve mai toccare il tasto elimina. **Testato 2026-09-17 (fix HUD a doppia variante, vedi nota in alto): funziona.** |
| 21 | Hall of Heroes | `[hallofheroestask]` | - | Per ogni eroe: sblocca tier gear T2/T3 se possibile, incanta gear T2/T3 (tutti gli eroi) + T1 (solo eroi nella formazione attiva) + tutti i jewel. **Punto critico da osservare**: verifica che il T1 venga incantato sugli eroi giusti (quelli davvero in formazione) - è l'assunzione meno sicura di tutto il codice, vedi `PLAN.md` Task 31. |
| 22 | Arena of Kings | `[arenaofkingstask]` | 80 | Sceglie l'avversario più debole tra i 3 mostrati, rerollando ogni 5s; dopo 3 min accetta fino a +5% di potenza, poi +10%, poi +20%, oltre i 9 min combatte comunque il migliore. Può girare a lungo (fino a 5 token/giorno) - non è un bug se impiega minuti. |

## Guild

| # | Nome | Sezione cfg | Livello min. | Comportamento atteso |
|---|------|--------------|:---:|----------------------|
| 23 | Expedition | `[expeditiontask]` | 10 | Reclama la spedizione attiva completata e ne avvia una nuova. |
| 24 | Tree of Life (Personal) | `[treeoflifetask]` | 10 | Gilda → Albero della Vita → vista Personale: compra upgrade con Expedition Token, priorità a Raining Gold/Firestone Finder/Firestone Effect. |
| 25 | Awakening | `[awakeningtask]` | 50 | Spende Arcane Crystal per risvegliare eroi, usando sempre il moltiplicatore più alto disponibile. **Testalo dopo Mailbox**, altrimenti probabilmente non ci sono cristalli da spendere. |

## Map & Warfront

| # | Nome | Sezione cfg | Livello min. | Comportamento atteso |
|---|------|--------------|:---:|----------------------|
| 26 | Map Missions | `[mapmissionstask]` | - | Reclama missioni completate, ne avvia di nuove nell'ordine configurato (`mission_time_order`, default `asc`). |
| 27 | Warfront Campaign Loot | `[warfrontcampaignloottask]` | 50 | Reclama i rotoli di ricompensa disponibili della campagna Warfront. |
| 28 | Warfront Daily Missions (Liberator) | `[warfrontdailymissionstask]` | 50 | Combatte le missioni di liberazione una a una, aspettando l'esito reale della battaglia prima di passare alla successiva. |

## Character

| # | Nome | Sezione cfg | Livello min. | Comportamento atteso |
|---|------|--------------|:---:|----------------------|
| 29 | Talents | `[talentstask]` | - | Character → tab Talenti: investe punti seguendo la sequenza guidata, salva dopo ogni nodo. Se il personaggio ha già più punti spesi della guida su un ramo, quel punto va semplicemente saltato (nessun errore atteso). |
| 30 | ⚠️ Path of Glory (Battle Pass) | `[pathofglorytask]` | - | Reclama le ricompense disponibili sia sulla traccia gratuita che su quella Golden (se posseduta) - non deve mai comprare il pass premium. **Testato 2026-09-17: path del bottone corretto (Mobile/Desktop, vedi nota in alto), ma il bottone risultava inattivo al momento del check - nessun claim osservato. Probabilmente non funziona ancora, da riverificare con un vero reward disponibile. Lasciato disabilitato.** |

---

## Cose da segnalare se succedono (non dovrebbero, ma sono i punti più a rischio)

- Qualsiasi spesa di **gemme** non prevista (in particolare durante BeerExchange).
- Un task che resta bloccato/non chiude mai la schermata che ha aperto (il Watchdog dovrebbe
  ripulire comunque entro `max_task_runtime`, ma se capita è un bug da segnalare).
- Hall of Heroes che potenzia il gear T1 su un eroe che *non* è nella formazione attiva.
- Qualunque click su un bottone di acquisto reale (a pagamento) invece che su un claim gratuito.

## Bonus volanti: confermati nel gioco, non ancora implementati

Il vecchio file di configurazione su questa macchina aveva una sezione `[flying_bonus_hunter]`
("Taps the flying dragon-with-beer and meteorite-hunter bonuses when they cross the screen") assente
dal codice attuale. Verificato direttamente sugli asset del gioco (non fidandosi del vecchio
riferimento): la feature esiste davvero, non è un residuo - `DragonWithBeer`, `FemaleDragonWithBeer` e
`MeteoriteHunter` sono tutti presenti, con mesh/renderer 3D propri (non UI a bottoni come il resto).
Non implementata: a differenza di ogni altro task, questi sembrano oggetti che attraversano la
schermata di battaglia dinamicamente, non un menu con un bottone - il meccanismo di spawn/click
richiede un'indagine dedicata (vedi `PLAN.md`, sezione "Punti aperti"). Non c'è nulla da testare per
questa feature nel giro attuale.
