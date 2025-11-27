# Gestor de Notas con MongoDB

## Capturas de Pantalla

### Vista Principal
<img width="1473" height="853" alt="image" src="https://github.com/user-attachments/assets/d1a40499-3275-40eb-9ad8-6f5ccb83542e" />

### Crear/Editar Nota
<img width="1474" height="853" alt="image" src="https://github.com/user-attachments/assets/e3cec9d5-6153-48e5-b97e-35c0281de884" />
<img width="1475" height="850" alt="image" src="https://github.com/user-attachments/assets/d7470aeb-ef5f-4e27-9a0b-7ff2a76ee2ce" />
<img width="1478" height="856" alt="image" src="https://github.com/user-attachments/assets/05eecaaa-500f-49a5-add3-ec7df525342e" />
<img width="1476" height="852" alt="image" src="https://github.com/user-attachments/assets/be05a9ea-5b7e-4cb2-83fc-9441a8934c3c" />

### MongoDB
<img width="1649" height="759" alt="image" src="https://github.com/user-attachments/assets/62ccf8fa-f484-4bcc-9cd0-986978c01d2b" />

## Instalación

### 1️- Clonar el Repositorio

### 2- Configurar MongoDB

- Entra a la pagina: https://cloud.mongodb.com/ y crea una cuenta gratis.
- Crea un nuevo cluster:
- Selecciona el tipo gratis que se llama M0
- Dale a crear
- Crea un usuario para la base de datos:
- Ve a donde dice "Database Access"
- Pulsa en "Add New Database User"
- Guarda bien el usuario y contraseña que pongas
- Para conectar con C#:
- Ve a "Database"
- Luego a "Clusters"
- Ahi veras tu base de datos
- Consigue tu cadena de conexion:
- Pulsa el boton "Connect"
- Elige "MongoDB for VS Code"
- Copia el texto que te aparece
- Cambia la parte donde dice tu usuario y tu contraseña por los que creaste
- Este texto lo vas a usar en el siguiente paso

### 3️- Configurar la Aplicación

#### Crear Archivo de JSON en ejectuble

1. Abre la carpeta en donde aparecen los paquetes de nuget y el .exe del form
2. Agrega ahí un archivo nombrado como "appsettings.json" 
3. Escribe en el archivo el json que esta a continuación pero reemplazando la ConnectionString por la tuya 

**Para MongoDB:**
```json
{
 "MongoDB": {
  "ConnectionString": "mongodb+srv://tuusuario:tupassword@cluster0.xxxxx.mongodb.net/",
  "DatabaseName": "GestorNotas",
  "CollectionName": "Notas"
  }
}
```

> Reemplaza `tuusuario`, `tupassword` y `cluster0.xxxxx` con tus credenciales reales de MongoDB Atlas
