"use client";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import type { Contact } from "@/types/domain/contact";
import { useDeleteContact } from "@/hooks/useContacts";

interface DeleteContactDialogProps {
  isOpen: boolean;
  onClose: () => void;
  contact: Contact | null;
  onSuccess: () => void;
}

export default function DeleteContactDialog({
  isOpen,
  onClose,
  contact,
  onSuccess,
}: DeleteContactDialogProps) {
  const deleteContact = useDeleteContact();

  const handleDelete = async () => {
    if (!contact) return;

    try {
      await deleteContact.mutateAsync(contact.id);
      onSuccess();
    } catch (error) {
      console.error("Error deleting contact:", error);
      onClose();
    }
  };

  return (
    <AlertDialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete Contact</AlertDialogTitle>
          <AlertDialogDescription>
            Are you sure you want to delete {contact?.name}? This action cannot
            be undone.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={deleteContact.isLoading}>
            Cancel
          </AlertDialogCancel>
          <AlertDialogAction
            onClick={handleDelete}
            disabled={deleteContact.isLoading}
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
          >
            {deleteContact.isLoading ? "Deleting..." : "Delete"}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
