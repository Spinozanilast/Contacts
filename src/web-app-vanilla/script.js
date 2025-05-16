class ContactAPI {
    constructor() {
        this.baseUrl = "http://localhost:5171/api/v1/contacts"
    }

    async _handleResponse(response) {
        if (!response.ok) {
            let errorMessage = `Error: ${response.status} ${response.statusText}`
            try {
                const errorData = await response.json()
                if (errorData.title || errorData.detail) {
                    errorMessage = errorData.title || errorData.detail
                } else if (errorData.errors) {
                    const errors = Object.values(errorData.errors).flat()
                    errorMessage = errors.join(", ")
                }
            } catch (e) {
                console.error(e)
            }
            throw new Error(errorMessage)
        }

        if (response.status === 204) {
            return null
        }

        return response.json()
    }

    async getContacts(pageNumber = 1, pageSize = 10) {
        const response = await fetch(`${this.baseUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`)
        return this._handleResponse(response)
    }

    async getContact(id) {
        const response = await fetch(`${this.baseUrl}/${id}`)
        return this._handleResponse(response)
    }

    async createContact(contactData) {
        const response = await fetch(this.baseUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(contactData),
        })

        if (response.status === 201) {
            const location = response.headers.get("Location")
            if (location) {
                const id = location.split("/").pop()
                return { id }
            }
        }

        return this._handleResponse(response)
    }

    async updateContact(id, contactData) {
        const response = await fetch(`${this.baseUrl}/${id}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(contactData),
        })

        return this._handleResponse(response)
    }

    async deleteContact(id) {
        const response = await fetch(`${this.baseUrl}/${id}`, {
            method: "DELETE",
        })

        return this._handleResponse(response)
    }

    async searchContacts(query, pageNumber = 1, pageSize = 10) {
        const response = await this.getContacts(pageNumber, pageSize)

        if (!query) return response

        const lowerQuery = query.toLowerCase()

        if (response && response.items) {
            response.items = response.items.filter(
                (contact) =>
                    contact.name.toLowerCase().includes(lowerQuery) ||
                    contact.mobilePhone.toLowerCase().includes(lowerQuery) ||
                    (contact.jobTitle && contact.jobTitle.toLowerCase().includes(lowerQuery)),
            )
        }

        return response
    }
}

class ContactManager {
    constructor() {
        this.api = new ContactAPI()
        this.contacts = []
        this.currentContactId = null
        this.currentPage = 1
        this.pageSize = 10
        this.totalPages = 1
        this.initElements()
        this.attachEventListeners()
        this.loadContacts()
    }

    initElements() {
        // Main elements
        this.contactsList = document.getElementById("contacts-list")
        this.searchInput = document.getElementById("search-input")

        // Pagination elements
        this.prevPageBtn = document.getElementById("prev-page-btn")
        this.nextPageBtn = document.getElementById("next-page-btn")
        this.pageInfoText = document.getElementById("page-info-text")
        this.pageSizeSelect = document.getElementById("page-size-select")

        // Modal elements
        this.contactModal = document.getElementById("contact-modal")
        this.deleteModal = document.getElementById("delete-modal")
        this.modalTitle = document.getElementById("modal-title")
        this.contactForm = document.getElementById("contact-form")
        this.contactIdInput = document.getElementById("contact-id")

        // Form inputs
        this.nameInput = document.getElementById("name")
        this.phoneInput = document.getElementById("phone")
        this.jobTitleInput = document.getElementById("jobTitle")
        this.birthdateInput = document.getElementById("birthdate")

        // Error messages
        this.nameError = document.getElementById("name-error")
        this.phoneError = document.getElementById("phone-error")

        // Buttons
        this.addContactBtn = document.getElementById("add-contact-btn")
        this.saveBtn = document.getElementById("save-btn")
        this.cancelBtn = document.getElementById("cancel-btn")
        this.confirmDeleteBtn = document.getElementById("confirm-delete-btn")
        this.cancelDeleteBtn = document.getElementById("cancel-delete-btn")

        // Toast
        this.toast = document.getElementById("toast")
        this.toastMessage = document.getElementById("toast-message")

        // Close modal buttons
        this.closeModalBtns = document.querySelectorAll(".close-modal")
    }

    attachEventListeners() {
        this.addContactBtn.addEventListener("click", () => this.openAddContactModal())

        this.contactForm.addEventListener("submit", (e) => this.handleFormSubmit(e))

        this.cancelBtn.addEventListener("click", () => this.closeModal(this.contactModal))

        this.confirmDeleteBtn.addEventListener("click", () => this.deleteContact())
        this.cancelDeleteBtn.addEventListener("click", () => this.closeModal(this.deleteModal))

        this.closeModalBtns.forEach((btn) => {
            btn.addEventListener("click", () => {
                this.closeModal(btn.closest(".modal"))
            })
        })

        window.addEventListener("click", (e) => {
            if (e.target === this.contactModal) {
                this.closeModal(this.contactModal)
            } else if (e.target === this.deleteModal) {
                this.closeModal(this.deleteModal)
            }
        })

        this.searchInput.addEventListener(
            "input",
            this.debounce(() => {
                this.searchContacts(this.searchInput.value)
            }, 300),
        )

        this.prevPageBtn.addEventListener("click", () => {
            if (this.currentPage > 1) {
                this.currentPage--
                this.loadContacts()
            }
        })

        this.nextPageBtn.addEventListener("click", () => {
            if (this.currentPage < this.totalPages) {
                this.currentPage++
                this.loadContacts()
            }
        })

        this.pageSizeSelect.addEventListener("change", () => {
            this.pageSize = Number.parseInt(this.pageSizeSelect.value)
            this.loadContacts()
        })
    }

    async loadContacts() {
        try {
            this.contactsList.innerHTML = '<div class="loading-indicator">Loading contacts...</div>'
            const response = await this.api.getContacts(this.currentPage, this.pageSize)

            console.log(response);

            this.totalPages = Math.ceil(response.totalCount / this.pageSize)
            this.updatePaginationControls()

            this.contacts = response.data || []
            this.renderContacts(this.contacts)
        } catch (error) {
            this.showToast(`Error loading contacts: ${error.message}`, "error")
            this.contactsList.innerHTML = '<div class="empty-state">Failed to load contacts. Please try again.</div>'
        }
    }

    updatePaginationControls() {
        this.pageInfoText.textContent = `Page ${this.currentPage} of ${this.totalPages || 1}`
        this.prevPageBtn.disabled = this.currentPage <= 1
        this.nextPageBtn.disabled = this.currentPage >= this.totalPages
    }

    renderContacts(contacts) {
        if (contacts.length === 0) {
            this.contactsList.innerHTML =
                '<div class="empty-state">No contacts found. Add a new contact to get started.</div>'
            return
        }

        this.contactsList.innerHTML = ""
        contacts.forEach((contact) => {
            const contactElement = document.createElement("div")
            contactElement.className = "contact-item"
            contactElement.innerHTML = `
                  <div class="contact-detail">${this.escapeHtml(contact.name)}</div>
                  <div class="contact-detail">${this.escapeHtml(contact.mobilePhone)}</div>
                  <div class="contact-detail hide-mobile">${this.escapeHtml(contact.jobTitle || "-")}</div>
                  <div class="contact-detail hide-mobile">${this.formatDate(contact.birthdate)}</div>
                  <div class="contact-actions">
                      <button class="icon-btn edit-btn" data-id="${contact.id}" title="Edit">✏️</button>
                      <button class="icon-btn delete-btn" data-id="${contact.id}" title="Delete">🗑️</button>
                  </div>
              `

            const editBtn = contactElement.querySelector(".edit-btn")
            const deleteBtn = contactElement.querySelector(".delete-btn")

            editBtn.addEventListener("click", () => this.openEditContactModal(contact.id))
            deleteBtn.addEventListener("click", () => this.openDeleteModal(contact.id))

            this.contactsList.appendChild(contactElement)
        })
    }

    openAddContactModal() {
        this.modalTitle.textContent = "Add Contact"
        this.contactForm.reset()
        this.contactIdInput.value = ""
        this.currentContactId = null
        this.clearFormErrors()
        this.openModal(this.contactModal)
    }

    async openEditContactModal(id) {
        try {
            const contact = await this.api.getContact(id)
            this.modalTitle.textContent = "Edit Contact"
            this.nameInput.value = contact.name
            this.phoneInput.value = contact.mobilePhone
            this.jobTitleInput.value = contact.jobTitle || ""
            this.birthdateInput.value = contact.birthdate ? contact.birthdate.split("T")[0] : ""
            this.contactIdInput.value = contact.id
            this.currentContactId = contact.id
            this.clearFormErrors()
            this.openModal(this.contactModal)
        } catch (error) {
            this.showToast(`Error loading contact: ${error.message}`, "error")
        }
    }

    openDeleteModal(id) {
        this.currentContactId = id
        this.openModal(this.deleteModal)
    }

    async handleFormSubmit(e) {
        e.preventDefault()

        if (!this.validateForm()) {
            return
        }

        const contactData = {
            name: this.nameInput.value.trim(),
            mobilePhone: this.phoneInput.value.trim(),
            jobTitle: this.jobTitleInput.value.trim() || null,
            birthdate: this.birthdateInput.value || null,
        }

        try {
            if (this.currentContactId) {
                await this.api.updateContact(this.currentContactId, contactData)
                this.showToast("Contact updated successfully", "success")
            } else {
                await this.api.createContact(contactData)
                this.showToast("Contact added successfully", "success")
            }

            this.closeModal(this.contactModal)
            this.loadContacts()
        } catch (error) {
            this.showToast(`Error saving contact: ${error.message}`, "error")
        }
    }

    async deleteContact() {
        try {
            await this.api.deleteContact(this.currentContactId)
            this.closeModal(this.deleteModal)
            this.showToast("Contact deleted successfully", "success")
            this.loadContacts()
        } catch (error) {
            this.showToast(`Error deleting contact: ${error.message}`, "error")
        }
    }

    async searchContacts(query) {
        try {
            this.contactsList.innerHTML = '<div class="loading-indicator">Searching...</div>'
            const response = await this.api.searchContacts(query, this.currentPage, this.pageSize)
            if (response.totalCount !== undefined) {
                this.totalPages = Math.ceil(response.totalCount / this.pageSize)
                this.updatePaginationControls()
            }

            this.contacts = response.items || []
            this.renderContacts(this.contacts)
        } catch (error) {
            this.showToast(`Error searching contacts: ${error.message}`, "error")
        }
    }

    validateForm() {
        let isValid = true
        this.clearFormErrors()

        if (!this.nameInput.value.trim()) {
            this.nameError.textContent = "Name is required"
            isValid = false
        }

        const phoneValue = this.phoneInput.value.trim()
        if (!phoneValue) {
            this.phoneError.textContent = "Phone number is required"
            isValid = false
        } else if (!this.isValidPhoneNumber(phoneValue)) {
            this.phoneError.textContent = "Please enter a valid phone number"
            isValid = false
        }

        return isValid
    }

    clearFormErrors() {
        this.nameError.textContent = ""
        this.phoneError.textContent = ""
    }

    isValidPhoneNumber(phone) {
        return /^[\d\s\-$$$$+]{7,20}$/.test(phone)
    }

    formatDate(dateString) {
        if (!dateString) return "-"

        try {
            const date = new Date(dateString)
            return date.toLocaleDateString()
        } catch (e) {
            return dateString
        }
    }

    showToast(message, type = "default") {
        this.toastMessage.textContent = message
        this.toast.className = "toast show"

        if (type) {
            this.toast.classList.add(type)
        }

        setTimeout(() => {
            this.toast.className = "toast"
        }, 3000)
    }

    openModal(modal) {
        modal.style.display = "block"
    }

    closeModal(modal) {
        modal.style.display = "none"
    }

    debounce(func, delay) {
        let timeout
        return function () {

            const args = arguments
            clearTimeout(timeout)
            timeout = setTimeout(() => func.apply(this, args), delay)
        }
    }

    escapeHtml(str) {
        if (!str) return ""
        return str
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;")
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new ContactManager();
});