// ===============================================
// profile.js
// Весь код обгорнуто у DOMContentLoaded
// ===============================================

document.addEventListener('DOMContentLoaded', () => {
    // 1. Отримання елементів DOM
    // Ці змінні стануть доступними лише після того, як DOM буде готовий.
    const editBtn = document.getElementById('editProfileBtn');
    const saveBtn = document.getElementById('saveProfileBtn');
    const cancelBtn = document.getElementById('cancelEditBtn');

    // Поля для редагування (View vs Edit)
    const editableFields = [
        { viewId: 'displayNameView', editId: 'displayNameEdit' },
        { viewId: 'emailView', editId: 'emailEdit' }
    ];

    // 2. Функція для перемикання режиму
    function toggleEditMode(isEditing) {
        // Запобігання помилці, якщо елемент не знайдено
        if (editBtn) editBtn.style.display = isEditing ? 'none' : 'flex';
        if (saveBtn) saveBtn.style.display = isEditing ? 'flex' : 'none';
        if (cancelBtn) cancelBtn.style.display = isEditing ? 'flex' : 'none';

        editableFields.forEach(field => {
            const viewElement = document.getElementById(field.viewId);
            const editElement = document.getElementById(field.editId);

            if (viewElement && editElement) {
                if (isEditing) {
                    // Копіюємо поточне значення з режиму перегляду в поле вводу
                    editElement.value = viewElement.textContent.trim();
                } else {
                    // Оновлюємо значення відображення при виході з режиму (для Cancel)
                    // При Save це вже буде оновлене значення
                    viewElement.textContent = editElement.value;
                }

                // Перемикаємо відображення
                viewElement.style.display = isEditing ? 'none' : 'flex';
                editElement.style.display = isEditing ? 'flex' : 'none';
            }
        });
    }

    // 3. Обробники подій

    // Додаємо обробники подій тільки якщо елементи були знайдені
    if (editBtn) {
        editBtn.addEventListener('click', () => {
            toggleEditMode(true);
        });
    }

    if (cancelBtn) {
        cancelBtn.addEventListener('click', () => {
            // Скасовуємо редагування
            toggleEditMode(false);
        });
    }

    if (saveBtn) {
        saveBtn.addEventListener('click', async () => {
            const newDisplayName = document.getElementById('displayNameEdit').value;
            const newEmail = document.getElementById('emailEdit').value;

            const dataToSend = {
                NewDisplayName: newDisplayName,
                NewEmail: newEmail
            };

            try {
                const response = await fetch('/Account/UpdateProfile', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(dataToSend)
                });

                const result = await response.json();

                if (response.ok) {
                    // Оновлюємо відображувані значення на основі відповіді сервера
                    document.getElementById('displayNameView').textContent = result.newDisplayName;
                    document.getElementById('emailView').textContent = result.newEmail;

                    // Виходимо з режиму редагування
                    toggleEditMode(false);
                    alert(result.message);
                } else {
                    alert('Failed to update profile: ' + (result.message || response.statusText));
                }
            } catch (error) {
                console.error('Error during profile update:', error);
                alert('An unexpected error occurred during profile update.');
            }
        });
    }

    // Встановлюємо початковий режим (перегляд) після завантаження
    toggleEditMode(false);
});