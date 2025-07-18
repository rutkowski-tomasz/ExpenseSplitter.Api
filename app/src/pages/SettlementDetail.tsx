import { useState, useEffect } from 'react';
import { ArrowLeft, Plus, Users, Share2, MoreVertical, DollarSign } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ExpenseCard } from '@/components/ExpenseCard';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

// Mock data for development
const mockSettlement = {
  id: '1',
  name: 'Weekend Trip',
  participants: [
    { id: '1', name: 'You', balance: 75.50 },
    { id: '2', name: 'Alice', balance: -25.00 },
    { id: '3', name: 'Bob', balance: -30.50 },
    { id: '4', name: 'Carol', balance: -20.00 },
  ],
  expenses: [
    {
      id: '1',
      name: 'Hotel Booking',
      amount: 240.00,
      paidBy: '1',
      paidByName: 'You',
      date: 'Mar 15, 2024',
      participantCount: 4,
      userShare: 60.00,
    },
    {
      id: '2',
      name: 'Gas Station',
      amount: 80.00,
      paidBy: '2',
      paidByName: 'Alice',
      date: 'Mar 14, 2024',
      participantCount: 4,
      userShare: 20.00,
    },
    {
      id: '3',
      name: 'Restaurant Dinner',
      amount: 130.00,
      paidBy: '3',
      paidByName: 'Bob',
      date: 'Mar 14, 2024',
      participantCount: 4,
      userShare: 32.50,
    },
  ],
  reimbursements: [
    { from: 'Alice', to: 'You', amount: 25.00 },
    { from: 'Bob', to: 'You', amount: 30.50 },
    { from: 'Carol', to: 'You', amount: 20.00 },
  ],
};

interface SettlementDetailProps {
  settlementId: string;
  onNavigate: (page: string, data?: any) => void;
}

export function SettlementDetail({ settlementId, onNavigate }: SettlementDetailProps) {
  const [settlement] = useState(mockSettlement);
  const [activeTab, setActiveTab] = useState('expenses');

  const totalExpenses = settlement.expenses.reduce((sum, exp) => sum + exp.amount, 0);

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <div className="bg-white border-b border-border p-4">
        <div className="flex items-center gap-3">
          <Button 
            variant="ghost" 
            size="icon"
            onClick={() => onNavigate('dashboard')}
            className="shrink-0"
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
          
          <div className="flex-1">
            <h1 className="text-xl font-bold text-foreground">{settlement.name}</h1>
            <p className="text-sm text-muted-foreground">{settlement.participants.length} participants</p>
          </div>
          
          <div className="flex items-center gap-2">
            <Button variant="ghost" size="icon">
              <Share2 className="w-5 h-5" />
            </Button>
            <Button variant="ghost" size="icon">
              <MoreVertical className="w-5 h-5" />
            </Button>
          </div>
        </div>
      </div>

      <div className="p-4 space-y-6">
        {/* Summary Card */}
        <Card className="shadow-card border-0">
          <CardContent className="p-6">
            <div className="text-center space-y-2">
              <div className="text-3xl font-bold text-foreground">${totalExpenses.toFixed(2)}</div>
              <div className="text-muted-foreground">Total group spending</div>
              
              <div className="flex items-center justify-center gap-4 mt-4">
                <div className="text-center">
                  <div className="text-lg font-semibold text-green-600">+$75.50</div>
                  <div className="text-xs text-muted-foreground">You are owed</div>
                </div>
                <div className="w-px h-8 bg-border" />
                <div className="text-center">
                  <div className="text-lg font-semibold text-foreground">{settlement.expenses.length}</div>
                  <div className="text-xs text-muted-foreground">Expenses</div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Quick Action */}
        <Button 
          onClick={() => onNavigate('addExpense', { settlementId })}
          className="w-full h-14"
        >
          <Plus className="mr-2 w-5 h-5" />
          Add New Expense
        </Button>

        {/* Tabs */}
        <Tabs value={activeTab} onValueChange={setActiveTab} className="space-y-4">
          <TabsList className="grid w-full grid-cols-3 bg-white rounded-xl shadow-card border-0">
            <TabsTrigger value="expenses" className="rounded-lg">Expenses</TabsTrigger>
            <TabsTrigger value="balances" className="rounded-lg">Balances</TabsTrigger>
            <TabsTrigger value="settle" className="rounded-lg">Settle Up</TabsTrigger>
          </TabsList>

          <TabsContent value="expenses" className="space-y-4">
            {settlement.expenses.map(expense => (
              <ExpenseCard
                key={expense.id}
                expense={expense}
                onClick={() => onNavigate('expenseDetail', { id: expense.id })}
              />
            ))}
          </TabsContent>

          <TabsContent value="balances" className="space-y-4">
            {settlement.participants.map(participant => (
              <Card key={participant.id} className="shadow-card border-0">
                <CardContent className="p-4">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <Avatar className="w-10 h-10">
                        <AvatarFallback className="bg-primary-light text-primary">
                          {participant.name.charAt(0).toUpperCase()}
                        </AvatarFallback>
                      </Avatar>
                      <div>
                        <div className="font-medium">{participant.name}</div>
                        <div className="text-sm text-muted-foreground">
                          {participant.balance > 0 ? 'Gets back' : participant.balance < 0 ? 'Owes' : 'Settled up'}
                        </div>
                      </div>
                    </div>
                    
                    <Badge 
                      variant={participant.balance > 0 ? "default" : participant.balance < 0 ? "destructive" : "secondary"}
                      className={`
                        font-medium rounded-full
                        ${participant.balance > 0 ? 'bg-green-100 text-green-700' : ''}
                        ${participant.balance < 0 ? 'bg-red-100 text-red-700' : ''}
                        ${participant.balance === 0 ? 'bg-gray-100 text-gray-700' : ''}
                      `}
                    >
                      {participant.balance !== 0 && (participant.balance > 0 ? '+' : '')}
                      ${Math.abs(participant.balance).toFixed(2)}
                    </Badge>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>

          <TabsContent value="settle" className="space-y-4">
            <Card className="shadow-card border-0">
              <CardHeader>
                <CardTitle className="text-lg">Suggested Payments</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {settlement.reimbursements.map((payment, index) => (
                  <div key={index} className="flex items-center justify-between p-3 bg-primary-light rounded-xl">
                    <div className="flex items-center gap-3">
                      <Avatar className="w-8 h-8">
                        <AvatarFallback className="text-xs bg-white text-primary">
                          {payment.from.charAt(0)}
                        </AvatarFallback>
                      </Avatar>
                      <div className="text-sm">
                        <span className="font-medium">{payment.from}</span>
                        <span className="text-muted-foreground"> pays </span>
                        <span className="font-medium">{payment.to}</span>
                      </div>
                    </div>
                    <div className="text-right">
                      <div className="font-bold">${payment.amount.toFixed(2)}</div>
                      <Button size="sm" variant="secondary" className="mt-1 h-6 text-xs">
                        Settle
                      </Button>
                    </div>
                  </div>
                ))}
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
}