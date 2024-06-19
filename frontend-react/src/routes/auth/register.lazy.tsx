import { createLazyFileRoute } from '@tanstack/react-router'

export const Route = createLazyFileRoute('/auth/register')({
  component: () => <div>Hello /auth/register!</div>
})