import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Index from "./pages/Index";
import { DashboardWrapper } from "./pages/DashboardWrapper";
import { SettlementDetailWrapper } from "./pages/SettlementDetailWrapper";
import { CreateExpenseWrapper } from "./pages/CreateExpenseWrapper";
import { ExpenseDetail } from "./pages/ExpenseDetail";
import { JoinSettlement } from "./pages/JoinSettlement";
import { CreateSettlement } from "./pages/CreateSettlement";
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Index />} />
          <Route path="/dashboard" element={<DashboardWrapper />} />
          <Route path="/settlement/:settlementId" element={<SettlementDetailWrapper />} />
          <Route path="/settlement/:settlementId/add-expense" element={<CreateExpenseWrapper />} />
          <Route path="/expense/:expenseId" element={<ExpenseDetail />} />
          <Route path="/join-settlement" element={<JoinSettlement />} />
          <Route path="/create-settlement" element={<CreateSettlement />} />
          {/* ADD ALL CUSTOM ROUTES ABOVE THE CATCH-ALL "*" ROUTE */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
