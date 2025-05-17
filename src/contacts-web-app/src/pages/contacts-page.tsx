"use client";

import { useState } from "react";
import { PlusIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import ContactTable from "@/components/contacts/contact-table";
import ContactFormDialog from "@/components/contacts/contact-form-dialog";
import DeleteContactDialog from "@/components/contacts/delete-contact-dialog";
import type { Contact } from "@/types/domain/contact";
import { useContacts } from "@/hooks/useContacts";

export default function ContactsPage() {
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [selectedContact, setSelectedContact] = useState<Contact | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [nameSearch, setNameSearch] = useState("");
  const [jobTitleSearch, setJobTitleSearch] = useState<string>("");

  const { data, isLoading, refetch } = useContacts({
    pageNumber: page,
    pageSize,
    nameSearch,
    jobTitleSearch,
  });

  const handleCreateContact = () => {
    setSelectedContact(null);
    setIsCreateDialogOpen(true);
  };

  const handleEditContact = (contact: Contact) => {
    setSelectedContact(contact);
    setIsEditDialogOpen(true);
  };

  const handleDeleteContact = (contact: Contact) => {
    setSelectedContact(contact);
    setIsDeleteDialogOpen(true);
  };

  const handleSearch = (name: string, jobTitle: string) => {
    setNameSearch(name);
    setJobTitleSearch(jobTitle);
    setPage(1); // Reset to first page when searching
  };

  return (
    <div className="container mx-auto py-8 px-4">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6 gap-4">
        <h1 className="text-3xl font-bold">Contact Manager</h1>
        <Button onClick={handleCreateContact} className="w-full md:w-auto">
          <PlusIcon className="mr-2 h-4 w-4" />
          Add Contact
        </Button>
      </div>

      <ContactTable
        contacts={data?.contacts.data || []}
        totalCount={data?.contacts.totalCount || 0}
        page={page}
        pageSize={pageSize}
        isLoading={isLoading}
        onPageChange={setPage}
        onPageSizeChange={setPageSize}
        onEdit={handleEditContact}
        onDelete={handleDeleteContact}
        onSearch={handleSearch}
        availableJobs={data?.jobs || []}
      />

      <ContactFormDialog
        isOpen={isCreateDialogOpen}
        onClose={() => setIsCreateDialogOpen(false)}
        onSuccess={() => {
          setIsCreateDialogOpen(false);
          refetch();
        }}
      />

      <ContactFormDialog
        isOpen={isEditDialogOpen}
        onClose={() => setIsEditDialogOpen(false)}
        contact={selectedContact}
        onSuccess={() => {
          setIsEditDialogOpen(false);
          refetch();
        }}
      />

      <DeleteContactDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => setIsDeleteDialogOpen(false)}
        contact={selectedContact}
        onSuccess={() => {
          setIsDeleteDialogOpen(false);
          refetch();
        }}
      />
    </div>
  );
}
