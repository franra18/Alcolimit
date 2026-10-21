# 🍺 Alcolimit

> **Calibra tu borrachera, engaña al alcoholímetro y huye de la policía hasta llegar a casa.**

**Alcolimit** es un videojuego 3D de simulación, exploración y conducción en primera persona desarrollado en **Unity** por **Dual Soul Games** para la asignatura de *Programación de Videojuegos*.

[![Ver tutorial en YouTube](https://img.shields.io/badge/▶_Ver_tutorial-YouTube-red?style=for-the-badge&logo=youtube)](https://youtu.be/j3t6Ko1aUPo)
![Unity](https://img.shields.io/badge/Unity-3D-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Plataforma](https://img.shields.io/badge/Plataforma-PC-blue?style=for-the-badge&logo=windows)

---

## 📑 Índice

- [Historia](#-historia)
- [Concepto de juego](#-concepto-de-juego)
- [Sistema de embriaguez](#-sistema-de-embriaguez)
- [Controles](#️-controles)
- [Escenarios y flujo de juego](#️-escenarios-y-flujo-de-juego)
- [Interfaz (HUD)](#️-interfaz-hud)
- [Diseño de arte y sonido](#-diseño-de-arte-y-sonido)
- [Programación](#-programación)
- [Estructura del proyecto](#-estructura-del-proyecto)
- [Assets de terceros](#-assets-de-terceros)
- [Uso de inteligencia artificial](#-uso-de-inteligencia-artificial)
- [Equipo](#-equipo)

---

## 📖 Historia

**La premisa.** Una noche desastrosa, demasiadas copas y un control policial masivo bloqueando la única salida. Estás atrapado.

**El giro.** Un rumor urbano resulta ser cierto: el alcoholímetro de la patrulla de guardia es un modelo obsoleto con un fallo de *firmware*. Si detecta un nivel de alcohol masivo y **extremadamente específico**, el sistema colapsa y marca **0.0**.

**El objetivo.** Vuelve al bar, calibra tu borrachera a ciegas para "hackear" el control y sobrevive a una persecución frenética en coche hasta llegar al garaje de tu casa.

### Personajes

- **El Conductor (protagonista):** un ciudadano anónimo que ha tenido una mala noche. No tiene líneas de diálogo, para maximizar la inmersión en primera persona.
- **El Policía:** la autoridad que custodia el control de tráfico. Estricto, pero limitado por un equipamiento obsoleto. Si no cumples el rango exacto, es *Game Over* instantáneo.

---

## 🎮 Concepto de juego

La partida se divide en dos fases a contrarreloj:

1. **🍸 Fase del Bar (infiltración y consumo).** Tienes **5 minutos** para explorar el bar, beber y ajustar tu nivel de alcohol al rango objetivo antes de salir voluntariamente o ser expulsado al acabarse el tiempo.
2. **🚗 Fase de Conducción (la huida).** Si superas el control policial, debes conducir borracho esquivando a la policía hasta llegar a casa. Si tu tasa no era la correcta, eres arrestado al instante.

---

## 🥴 Sistema de embriaguez

El estado del jugador se rige por una variable interna de alcohol de **0 a 10**:

- **Nivel inicial:** empiezas con un nivel de **1** y nunca puedes bajar de ahí.
- **Rango defectuoso:** al inicio de cada partida se elige aleatoriamente el rango en el que fallará el alcoholímetro: **5‑6, 6‑7, 7‑8 u 8‑9**. Se muestra en una viñeta introductoria y permanece visible en el HUD durante la fase del bar.
- **Ceguera de interfaz:** conoces el rango objetivo, pero **no tu nivel exacto**. Tienes que guiarte por los síntomas de tu personaje:
  - **Distorsión de cámara:** balanceo y cabeceo constantes que simulan la pérdida de equilibrio.
  - **Filtro cromático:** un tono verdoso que se intensifica con cada nivel de alcohol.

### Acciones para regular el alcohol

| Acción | Dónde | Efecto |
|---|---|---|
| 🍾 **Beber** | Botellas repartidas por el bar | Sube la tasa |
| 🚽 **Orinar** | Baños de la planta superior | Baja la tasa de forma moderada |
| 💃 **Bailar** | Pista de baile | Baja la tasa de forma moderada |
| 🤮 **Vomitar** | Lavabo del baño (solo con nivel críticamente alto) | Baja la tasa drásticamente |

---

## ⌨️ Controles

| Entrada | Fase Bar | Fase Coche | Comportamiento |
|---|---|---|---|
| `W` `A` `S` `D` | Movimiento | Dirección | Desplazamiento del personaje / control del volante |
| `Ratón` | Mirar | Mirar | Orientación de la cámara en primera persona |
| `E` | Interactuar | – | Beber, orinar, bailar o vomitar cerca del objeto |
| `W` | – | Acelerar | Avanzar con el vehículo |
| `S` | – | Frenar / marcha atrás | Reducir velocidad o retroceder |
| `Espacio` | – | Freno de mano | Freno de emergencia y derrape |

---

## 🗺️ Escenarios y flujo de juego

### 1. El Bar (interior de dos plantas)
- **Planta baja:** salón abierto con mesas altas, una barra lineal a lo largo de la pared trasera y una pista de baile con baldosas de colores bajo una bola de discoteca.
- **Planta superior:** una **Sala VIP** con sofás, mesas bajas y tocadiscos, y una zona de **aseos** (antesala con lavabo y cubículo con inodoro), clave para reducir el alcohol.
- El aviso contextual *"Pulsa [E] para..."* solo aparece al acercarse a los puntos de interés.

### 2. Transición: el Control Policial
Escena cinematográfica en la que se evalúa tu nivel. Si estás dentro del rango, el alcoholímetro marca absurdamente **0.0** y puedes continuar.

### 3. La Carretera (la persecución)
Ciudad nocturna con bloques de edificios, farolas, mobiliario urbano y tráfico. Conduces en primera persona desde la cabina, conservando el filtro verdoso y el tambaleo acumulados en el bar.

**🏁 Condición de victoria:** llegar al garaje de tu casa sin ser interceptado.

---

## 🖥️ Interfaz (HUD)

Interfaz minimalista que cambia según la fase:

**Fase Bar**
- **Rango objetivo** (arriba a la izquierda), p. ej. *"Rango objetivo: Entre 5 y 6"*.
- **Barra de alcohol** (arriba en el centro), de 0 a 10, que solo muestra los extremos numéricos.
- **Temporizador** (arriba a la derecha), cuenta atrás desde `05:00`.
- **Retícula central** para apuntar a botellas y elementos interactivos.

**Fase Carretera**
- **Distancia policial** (arriba en el centro), p. ej. *"Policía a: 28,2 m"*.
- **Minimapa dinámico** (abajo a la izquierda) con las calles y la posición del coche de policía.
- **Flecha magenta** junto al minimapa que apunta siempre hacia tu casa.
- **Velocímetro y marchas** integrados en el salpicadero del coche.

---

## 🎨 Diseño de arte y sonido

- **Estilo:** 3D *low-poly* de aristas marcadas, geometrías puras y texturas limpias. Referencia visual: *Gamble With Your Friends*.
- **Bar:** iluminación uniforme y cálida que transmite una falsa calma, en contraste con el filtro verdoso y el tambaleo de la embriaguez.
- **Carretera:** iluminación urbana más lúgubre, rota por los destellos rojos y azules de las sirenas.
- **Sonido:** música electrónica y rítmica en el bar; motor del coche y sirenas policiales durante la persecución.

---

## 💻 Programación

- **Embriaguez visual procedural.** El mareo de la cámara no usa animaciones pregrabadas: se calcula con `Mathf.PerlinNoise` para lograr un movimiento orgánico que escala con el nivel de alcohol. Ese mismo valor modifica en tiempo real el *Post-Processing* de Unity (distorsión de lente y color).
- **NPCs con NavMesh Surface y zonas de interés.** Los NPCs evalúan qué ubicaciones están libres (barra, pista de baile...) para elegir su siguiente destino y calculan la ruta óptima esquivando obstáculos.
- **Circulación de coches.** El tráfico sigue 3 rutas cíclicas para repartirse por todo el mapa. Se usan *Raycasts* y semáforos en las zonas críticas para evitar bloqueos por choques frontales.
- **IA de persecución policial.** El coche de policía actualiza continuamente su destino hacia el jugador usando NavMesh. Si te mantiene en su rango de captura el tiempo suficiente, se detiene la partida y aparece la pantalla de derrota.

---

## 📁 Estructura del proyecto

```
Assets/
├── Scenes/
│   ├── MenuInicio.unity        # Menú principal
│   ├── Introduccion.unity      # Viñeta introductoria con el rango objetivo
│   ├── Interior.unity          # Fase 1: el bar
│   ├── ControlPolicia.unity    # Transición: control de alcoholemia
│   └── Exterior.unity          # Fase 2: persecución por la ciudad
├── Scripts/                    # Lógica del juego (C#)
│   ├── GameManager.cs, SistemaAlcohol.cs, TemporizadorNivel.cs
│   ├── FirstPersonController.cs, PlayerInteraction.cs, BottleInteractable.cs
│   ├── ZonaAlivio.cs, ZonaBaileJugador.cs, NPCMovimiento.cs, ZonaNPC.cs
│   ├── PoliciaIA.cs, GestorPersecucion.cs, CocheAleatorio.cs, SemaforoCritico.cs
│   └── CamaraMinimapa.cs, IndicadorMinimapa.cs, EfectosAlcoholCoche.cs, ...
├── Prefabs/ · Materials/ · Images/ · Sprites/ · Sounds/
└── PrefabsPackages/            # Assets de terceros
```

### ▶️ Cómo abrir el proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/franra18/Alcolimit.git
   ```
2. Abre la carpeta con **Unity Hub** (o importa el `.unitypackage` del proyecto en un proyecto 3D nuevo).
3. Abre la escena `Assets/Scenes/MenuInicio.unity` y pulsa **Play**.

---

## 📦 Assets de terceros

| Asset | Autor | Uso |
|---|---|---|
| **SimplePoly City** | VenCreations | Entorno urbano modular con carreteras y tráfico |
| **Interiors FREE** | Mnostva Art | Interior minimalista del bar |
| **Creative Characters FREE** | ithappy | Modelos de clientes y NPCs |
| **Ezereal Car Controller** | Ezereal | Controlador del vehículo |
| **Farland Skies – Low Poly** | Borodar | Cielos *low-poly* |

---

## 🤖 Uso de inteligencia artificial

- **Gemini:** optimización y depuración de scripts (circulación de coches, seguimiento del coche de policía), apoyo en dirección de arte y audio, y creación de la pantalla de inicio.
- **Unity AI:** generación de *prefabs* (botellas y mobiliario del baño) y organización de escenas y colocación de elementos en el bar.

---

## 👥 Equipo

**Dual Soul Games**

| Integrante | Roles |
|---|---|
| **Francisco Ramírez Cañadas** | Diseño de juego, Arte 3D/2D, Diseño de niveles y Producción |
| **Marcos Luque Montiel** | Diseño de juego, Programación, Interfaz/UI y Diseño de niveles |
| **Luis Fernández Rodríguez** | Diseño de juego, Programación, Interfaz/UI y Sonido |

---

> ⚠️ *Alcolimit es una obra de ficción con tono humorístico. No bebas si vas a conducir.*
