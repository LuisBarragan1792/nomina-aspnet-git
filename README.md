# Sistema de Nómina Web - ASP.NET Core

## Descripción

Este proyecto consiste en un sistema web de gestión de nómina desarrollado en **ASP.NET Core MVC** utilizando **Entity Framework Core** y **SQL Server**.

El sistema permite administrar empleados, departamentos y salarios, además de registrar auditorías de cambios realizados en los salarios.

Este proyecto fue desarrollado como parte de una práctica académica para aplicar control de versiones con **Git y GitHub**, implementación de **MVC**, y manejo de **bases de datos relacionales**.

---

## Tecnologías utilizadas

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- Bootstrap
- Git
- GitHub

---

## Funcionalidades principales

### Gestión de empleados
Permite registrar y visualizar empleados dentro del sistema.

### Gestión de departamentos
Permite crear y administrar departamentos.

### Gestión de salarios
Permite registrar salarios asociados a un empleado, validando:

- Existencia del empleado
- Coherencia de fechas
- No solapamiento de salarios

### Auditoría de salarios
Cada vez que se registra un salario, el sistema guarda un registro de auditoría que incluye:

- Usuario que realizó la acción
- Fecha de modificación
- Salario registrado
- Empleado afectado
- Detalle del cambio

### Dashboard
El sistema cuenta con un panel principal que muestra:

- Total de empleados
- Total de departamentos
- Salarios vigentes
- Salarios próximos a vencer

---

## Control de versiones

El proyecto utiliza **Git** como sistema de control de versiones y **GitHub** como repositorio remoto.

Se trabajó utilizando ramas de desarrollo para implementar nuevas funcionalidades.

Ejemplo de rama utilizada:
