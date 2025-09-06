using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Kontecg.Logging;
using Microsoft.Win32;

namespace Kontecg.Runtime
{
    public static class ScreenManager
    {
        private static Screen _currentScreen;

        public static Screen CurrentScreen
        {
            get => _currentScreen ??= LoadScreen();
            set
            {
                _currentScreen = value;
                SaveScreen(value);
            }
        }

        // Inicializar el gestor de pantallas
        public static void Initialize()
        {
            // Suscribirse a eventos de cambio de configuración de pantalla
            SystemEvents.DisplaySettingsChanged += (s, e) =>
            {
                // Revalidar la pantalla guardada cuando cambia la configuración
                var savedScreen = LoadScreen();
                if (savedScreen != null)
                {
                    _currentScreen = savedScreen;
                }
            };
        }

        // Cargar la configuración de pantalla guardada
        private static Screen LoadScreen()
        {
            try
            {
                string savedScreenId = WinFormsRuntimeContext.Display;
                if (string.IsNullOrEmpty(savedScreenId))
                    return Screen.PrimaryScreen;

                // Buscar la pantalla por el ID guardado
                var allScreens = Screen.AllScreens;
                foreach (var screen in allScreens)
                {
                    if (GetScreenUniqueId(screen) == savedScreenId)
                        return screen;
                }

                // Si no se encuentra, usar la primaria
                return Screen.PrimaryScreen;
            }
            catch
            {
                return Screen.PrimaryScreen;
            }
        }

        // Guardar la configuración de pantalla
        private static void SaveScreen(Screen screen)
        {
            try
            {
                string screenId = GetScreenUniqueId(screen);
                WinFormsRuntimeContext.Display = screenId;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        // Generar un ID único para la pantalla
        public static string GetScreenUniqueId(Screen screen)
        {
            return $"{screen.DeviceName}_{screen.Bounds.Width}_{screen.Bounds.Height}";
        }

        // Obtener todas las pantallas disponibles
        public static List<Screen> GetAllScreens()
        {
            return Screen.AllScreens.ToList();
        }

        // Establecer un formulario en la pantalla actual
        public static void SetFormToCurrentScreen(Form form)
        {
            if (form == null || CurrentScreen == null) return;

            try
            {
                FormWindowState oldSate = form.WindowState;
                
                // Asegurar que el formulario no esté minimizado al cambiar de pantalla
                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;

                // Establecer la ubicación en la pantalla actual
                form.StartPosition = FormStartPosition.Manual;
                form.Location = CurrentScreen.Bounds.Location;

                // Para formularios DevExpress, podemos usar características específicas
                if (form is XtraForm xtraForm)
                {
                    // Asegurar que el formulario se muestre correctamente en la pantalla
                    xtraForm.DesktopLocation = CurrentScreen.Bounds.Location;
                }

                form.WindowState = oldSate;
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }
    }
}