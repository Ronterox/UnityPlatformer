# Unity 3D Parkour Platformer 🌟

- **Sistema de Parkour Avanzado**: Movimiento fluido que incluye correr, saltar y una mecánica de escalada intuitiva.
- **Mecánica de Escalada (Roblox Style)**: Los jugadores pueden aferrarse a las superficies y escalar estructuras verticales, permitiendo un diseño de niveles más vertical.
- **Coleccionables**: Sistema de estrellas distribuidas estratégicamente por el mapa. ¡Solo los más hábiles podrán conseguirlas todas!
- **Cámara en 3ra Persona**: Una cámara optimizada para seguir la acción y facilitar los saltos de precisión.

### 1. Sistema de Movimiento del Personaje
Utilicé un `CharacterController` para gestionar las colisiones y el movimiento físico. La lógica principal permite:
- **Detección de Suelo**: Para evitar saltos infinitos y aplicar gravedad realista.
- **Input Responsivo**: Configurado para ofrecer esa sensación de control total necesaria en juegos de parkour.

### 2. Algoritmo de Escalada
Para replicar el estilo de escalada de Roblox, implementé un sistema de *Raycasting*:
- El script lanza rayos hacia adelante para detectar si hay una pared u objeto "escalable".
- Al detectar la pared, se anula la gravedad temporalmente y se cambia la animación para permitir el desplazamiento vertical.

### 3. Lógica de Recolección (Stars)
Cada estrella es un **Prefab** con un script que gestiona:
- **Rotación y Animación**: Un efecto visual continuo para hacerlas atractivas.
- **Triggers**: Uso de `OnTriggerEnter` para detectar cuando el jugador las toca, sumarlas al contador y destruirlas de la escena con un efecto de sonido/partículas.

### 4. Diseño de Niveles
Los niveles están diseñados para fomentar la verticalidad. Utilicé herramientas de prototipado rápido para asegurar que cada salto sea posible pero desafiante.

## 📦 Instalación y Uso

1. **Clonar el repositorio**:
   ```bash
   git clone [https://github.com/Ronterox/UnityPlatformer.git](https://github.com/Ronterox/UnityPlatformer.git)
