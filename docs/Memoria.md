# Memoria del Proyecto: ShinyVillage

**Asignatura:** Proyecto Intermodular DAM
**Alumno:** Cristina García Quintero
**Fecha:** Marzo 2026  
**Repositorio:** [URL del repositorio GitHub](https://github.com/little-shiny/ShinyVillage)

---
aqui el resumen en español y en inglés

---

# 1. Planificación

## 1.1 Descripción del proyecto

**ShinyVillage** es un videojuego de rol en dos dimensiones (RPG 2D) que fue creado con el motor Unity y cuya programación se realizó en C#. La propuesta principal es poner al jugador en la dirección de una aldea, donde tiene que administrar los recursos, cultivar su granja y examinar el ambiente desde un punto de vista cenital típico del género.

El jugador maneja a un personaje que se mueve sin restricciones por el mapa, recoge objetos del entorno, los guarda en un inventario y realiza acciones con aquellos elementos del terreno que son interactivos, como las celdas agrícolas. El personaje es seguido de manera constante por la cámara. Como el progreso se almacena en una base de datos local (SQLite), el jugador tiene la posibilidad de crear múltiples partidas independientes, reanudarlas cuando quiera o eliminarlas desde el menú.

El ciclo de juego proyectado incorpora procedimientos relacionados con la agricultura (cultivar, regar y recoger productos en tiles), manejo del inventario (recoger, amontonar y soltar artículos) e interacción con el mapa a través de un sistema de tiles clasificados según su condición.

## 1.2 Contexto del proyecto
| **Aspecto** | Implementación |
|---|---|
| **Ámbito y entorno** | El proyecto se desarrolla en un contexto académico, sin un cliente externo, donde el propio equipo actúa como desarrollador y receptor técnico del producto. El entorno de desarrollo se basa en **Unity** con **C#**, utilizando **SQLite** integrado mediante **NuGetForUnity** para la persistencia de datos, mientras que el control de versiones se gestiona con **Git** mediante ramas de desarrollo. El proyecto parte desde cero con el objetivo de aprender desarrollo de videojuegos y aplicar una arquitectura sólida. Para ello se plantean como elementos clave un sistema de guardado seguro del estado de la partida, un inventario con gestión visual de espacios, movimiento del personaje con animaciones fluidas, un sistema de losetas interactivas basado en **Tilemap** y una estructura escalable que permita añadir nuevas mecánicas sin modificar el núcleo del sistema. |
| **Solución y justificación** | La solución elegida estructura el juego en sistemas independientes conectados mediante patrones de diseño. Se utiliza el patrón **Singleton** en gestores como `GameManager`, `DatabaseManager` y `SaveGameManager` para garantizar una única instancia global en **Unity**. La persistencia se gestiona con **SQLite**, lo que permite organizar los datos de forma relacional y facilitar consultas y ampliaciones del sistema. Además, el patrón **Repository** separa el acceso a la base de datos de la lógica del juego.Por otro lado, el inventario se implementa con una clase independiente del motor y una interfaz de usuario separada para la visualización. Finalmente, el sistema de tiles utiliza el componente **Tilemap**, gestionado por un `TileManager`. Todo ello sigue una arquitectura en capas que separa presentación, lógica y datos para mejorar la mantenibilidad y escalabilidad del proyecto. |
| **Destinatarios** | El juego contempla como único tipo de usuario el jugador. Su rol dentro del sistema le permite crear y modificar partidas desde el menú, así como el movimiento de un personaje y sus decisiones sobre la interacción con el mapa. Tmbién puede gestionar su inventario decidiendo qué objetos soltar y cuáles soltar. No se contempla ningún rol de administrador ni perfil técnico dentro de la experiencia de juego. Toda la gestión interna (base de datos, repositorios, gestores) es transparente para el jugador y opera de forma automática en segundo plano. |



## 1.3 Objetivo del proyecto

ShinyVillage tiene la finalidad de ofrecer al jugador una experiencia de ocio digital tranquilo, de estilo casual basado en una siple gestión de recursos y un entorno de fantasía. 
La aplicación no tiene una vocación comercial en este momento porque está enmarcada en un contecto de aprendizaje y desarrollo en técnicas de programación de videojuegos.

## 1.4 Marco legal
Al tratarse de un videojuego de entretenimiento en fase de desarrollo académico, sin distribución comercial ni recogida de datos personales identificativos, el marco legal aplicable es reducido.

### Protección de datos (RGPD / LOPDGDD)
La aplicación almacena únicamente datos de juego —nombre de personaje, progreso, posición— de forma local en el dispositivo del usuario, sin transmisión a servidores externos. El nombre del personaje es un alias libremente elegido, no vinculado a ninguna identidad real, por lo que en su estado actual no se tratan datos personales en el sentido del RGPD (Reglamento UE 2016/679) ni de la LOPDGDD (LO 3/2018). Si en el futuro se incorporasen cuentas de usuario o cualquier dato identificativo, la aplicación quedaría sujeta a dichas normas.

### Propiedad intelectual

El proyecto ShinyVillage se distribuye bajo una **licencia personalizada basada en Apache License 2.0**, a la que se añade una cláusula de restricción de uso comercial. Esto significa que cualquier persona puede usar, estudiar, modificar y redistribuir el software y sus versiones derivadas, pero **no puede hacerlo con fines comerciales** sin autorización expresa.

Esta licencia no está aprobada por la OSI (*Open Source Initiative*) al incluir dicha restricción, pero es plenamente válida desde el punto de vista legal.

Si en algún momento se desea autorizar un uso comercial concreto a un tercero, bastaría con un acuerdo escrito adicional entre las partes, sin necesidad de cambiar esta licencia.

### Clasificación por edades
En caso de distribución pública, el sistema PEGI clasificaría previsiblemente el juego como **PEGI 3**, dado su contenido de fantasía sin elementos violentos ni adultos.


## 1.5 Requisitos funcionales y no funcionales



# 7. Referencias

**Protección de datos y privacidad**
- [Reglamento (UE) 2016/679 — Reglamento General de Protección de Datos (RGPD)](https://eur-lex.europa.eu/legal-content/ES/TXT/?uri=CELEX%3A32016R0679)
- [Ley Orgánica 3/2018, de Protección de Datos Personales y garantía de los derechos digitales (LOPDGDD)](https://www.boe.es/buscar/act.php?id=BOE-A-2018-16673)
- [Agencia Española de Protección de Datos — Guía para el cumplimiento del RGPD](https://www.aepd.es/guias/guia-rgpd-para-responsables-de-tratamiento.pdf)

**Propiedad intelectual**
- [Real Decreto Legislativo 1/1996 — Ley de Propiedad Intelectual](https://www.boe.es/buscar/act.php?id=BOE-A-1996-8930)

**Clasificación por edades**
- [Sistema de clasificación PEGI — Pan European Game Information](https://pegi.info/es)

**Licencias de software**
- [Apache License, Version 2.0 — texto oficial](https://www.apache.org/licenses/LICENSE-2.0)
- [Open Source Initiative — definición de licencia de código abierto](https://opensource.org/osd)

**Tecnologías utilizadas**
- [Unity — documentación oficial](https://docs.unity3d.com/Manual/index.html)
- [SQLite — documentación oficial](https://www.sqlite.org/docs.html)
- [System.Data.SQLite — documentación del paquete](https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki)
- [Patrón Singleton en Unity — Game Programming Patterns](https://gameprogrammingpatterns.com/singleton.html)