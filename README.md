# IA

Nombre del Juego: 

Género: Sigilo

Objetivo: Recolectar objetos y llevarlos a la zona de entrega sin ser atrapado por el enemigo, utlizando los arboles para esconderse. 
Al completar la cantidad requerida aparece la pantalla de victoria. Si el enemigo te alcanza, aparece la pantalla de derrota.

Sistema de IA y Controles: 

El enemigo cuenta con una máquina de estados finitos (FSM) con cuatro estados. En Patrol recorre una serie de waypoints en loop. 
Si detecta al jugador, pasa a Pursuit y lo persigue. La detección se basa en tres condiciones simultáneas: que el jugador esté dentro de un rango de distancia, dentro del ángulo de visión del enemigo, y que no haya obstáculos entre ambos. 
Si pierde de vista al jugador, pasa a Idle donde espera unos segundos girando sobre sí mismo antes de retomar la patrulla. 
Por último, si el jugador agarra el objeto especial y está dentro del rango de detección del enemigo, este entra en estado Flee y huye en dirección contraria durante unos segundos, volviendo luego a patrullar.

Controles

WASD — moverse
Mouse — mirar alrededor
E — agarrar o soltar un objeto / interactuar con objetos del mundo
Escape — cerrar panel de información