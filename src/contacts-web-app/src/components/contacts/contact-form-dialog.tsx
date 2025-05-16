"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import type { Contact } from "@/types/domain/contact";
import { useCreateContact, useUpdateContact } from "@/hooks/useContacts";
import { ContactSchema } from "@/types/zod-schemas";

type FormValues = z.infer<typeof ContactSchema>;

interface ContactFormDialogProps {
  isOpen: boolean;
  onClose: () => void;
  contact?: Contact | null;
  onSuccess: () => void;
}

export default function ContactFormDialog({
  isOpen,
  onClose,
  contact,
  onSuccess,
}: ContactFormDialogProps) {
  const isEditMode = !!contact;
  const createContact = useCreateContact();
  const updateContact = useUpdateContact();

  const form = useForm<FormValues>({
    resolver: zodResolver(ContactSchema),
    defaultValues: {
      name: "",
      mobilePhone: "",
      jobTitle: "",
      birthDate: "",
    },
  });

  useEffect(() => {
    if (contact) {
      form.reset({
        name: contact.name,
        mobilePhone: contact.mobilePhone,
        jobTitle: contact.jobTitle || "",
        birthDate: contact.birthDate
          ? new Date(contact.birthDate).toISOString().split("T")[0]
          : "",
      });
    } else {
      form.reset({
        name: "",
        mobilePhone: "",
        jobTitle: "",
        birthDate: "",
      });
    }
  }, [contact, form]);

  const onSubmit = async (data: FormValues) => {
    try {
      if (isEditMode && contact) {
        await updateContact.mutateAsync({
          id: contact.id,
          ...data,
        });
      } else {
        await createContact.mutateAsync(data);
      }
      onSuccess();
    } catch (error) {
      console.error("Error saving contact:", error);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>
            {isEditMode ? "Edit Contact" : "Add Contact"}
          </DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name</FormLabel>
                  <FormControl>
                    <Input placeholder="John Doe" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="mobilePhone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Mobile Phone</FormLabel>
                  <FormControl>
                    <Input placeholder="+1 (555) 123-4567" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="jobTitle"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Job Title</FormLabel>
                  <FormControl>
                    <Input placeholder="Software Engineer" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="birthDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Birth Date</FormLabel>
                  <FormControl>
                    <Input type="date" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" onClick={onClose}>
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={createContact.isLoading || updateContact.isLoading}
              >
                {createContact.isLoading || updateContact.isLoading
                  ? "Saving..."
                  : "Save"}
              </Button>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
