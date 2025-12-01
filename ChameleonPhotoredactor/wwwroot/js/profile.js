
document.addEventListener('DOMContentLoaded', () => {

    const editBtn = document.getElementById('editProfileBtn');
    const saveBtn = document.getElementById('saveProfileBtn');
    const cancelBtn = document.getElementById('cancelEditBtn');
    const deleteBtn = document.getElementById('deleteAccount');
    const cameraiconBtn = document.getElementById('cameraIconContainer');

    const editableFields = [
        { viewId: 'displayNameView', editId: 'displayNameEdit' },
        { viewId: 'emailView', editId: 'emailEdit' }
    ];

    function toggleEditMode(isEditing) {
        if (editBtn) editBtn.style.display = isEditing ? 'none' : 'flex';
        if (saveBtn) saveBtn.style.display = isEditing ? 'flex' : 'none';
        if (cancelBtn) cancelBtn.style.display = isEditing ? 'flex' : 'none';

        if (deleteBtn) {
            if (isEditing) {
                deleteBtn.classList.add('show');
            } else {
                deleteBtn.classList.remove('show');
            }
        }

        if (cameraiconBtn) {
            if (isEditing) {
                cameraiconBtn.classList.add('show');
            } else {
                cameraiconBtn.classList.remove('show');
            }
        }

        editableFields.forEach(field => {
            const viewElement = document.getElementById(field.viewId);
            const editElement = document.getElementById(field.editId);

            if (viewElement && editElement) {
                if (isEditing) {
                    editElement.value = viewElement.textContent.trim();
                } else {
                    viewElement.textContent = editElement.value;
                }

                viewElement.style.display = isEditing ? 'none' : 'flex';
                editElement.style.display = isEditing ? 'flex' : 'none';
            }
        });
    }

    if (editBtn) {
        editBtn.addEventListener('click', () => {
            toggleEditMode(true);
        });
    }

    if (cancelBtn) {
        cancelBtn.addEventListener('click', () => {
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
                    document.getElementById('displayNameView').textContent = result.newDisplayName;
                    document.getElementById('emailView').textContent = result.newEmail;

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

    toggleEditMode(false);
});