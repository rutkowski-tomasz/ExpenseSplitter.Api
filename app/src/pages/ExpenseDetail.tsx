import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Calendar, DollarSign, Users, Edit, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';

// Mock data for development
const mockExpense = {
  id: '1',
  name: 'Hotel Booking',
  amount: 240.00,
  paidBy: '1',
  paidByName: 'You',
  date: 'Mar 15, 2024',
  participantCount: 4,
  userShare: 60.00,
  settlementId: '1',
  settlementName: 'Weekend Trip',
  allocations: [
    { participantId: '1', participantName: 'You', amount: 60.00 },
    { participantId: '2', participantName: 'Alice', amount: 60.00 },
    { participantId: '3', participantName: 'Bob', amount: 60.00 },
    { participantId: '4', participantName: 'Carol', amount: 60.00 },
  ]
};

export function ExpenseDetail() {
  const { expenseId } = useParams<{ expenseId: string }>();
  const navigate = useNavigate();
  const [expense] = useState(mockExpense);

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <div className="bg-white border-b border-border p-4">
        <div className="flex items-center gap-3">
          <Button 
            variant="ghost" 
            size="icon"
            onClick={() => navigate(`/settlement/${expense.settlementId}`)}
            className="shrink-0"
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
          
          <div className="flex-1">
            <h1 className="text-xl font-bold text-foreground">{expense.name}</h1>
            <p className="text-sm text-muted-foreground">{expense.settlementName}</p>
          </div>
          
          <div className="flex items-center gap-2">
            <Button variant="ghost" size="icon">
              <Edit className="w-5 h-5" />
            </Button>
            <Button variant="ghost" size="icon">
              <Trash2 className="w-5 h-5" />
            </Button>
          </div>
        </div>
      </div>

      <div className="p-4 space-y-6">
        {/* Expense Summary */}
        <Card className="shadow-card border-0">
          <CardContent className="p-6">
            <div className="text-center space-y-4">
              <div className="text-4xl font-bold text-foreground">${expense.amount.toFixed(2)}</div>
              <div className="text-muted-foreground">Total amount</div>
              
              <div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
                <Calendar className="w-4 h-4" />
                <span>{expense.date}</span>
              </div>
              
              <div className="flex items-center justify-center gap-4 pt-4 border-t border-border">
                <div className="text-center">
                  <div className="text-lg font-semibold text-primary">${expense.userShare.toFixed(2)}</div>
                  <div className="text-xs text-muted-foreground">Your share</div>
                </div>
                <div className="w-px h-8 bg-border" />
                <div className="text-center">
                  <div className="text-lg font-semibold text-foreground">{expense.participantCount}</div>
                  <div className="text-xs text-muted-foreground">People</div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Paid By */}
        <Card className="shadow-card border-0">
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">Paid by</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex items-center gap-3">
              <Avatar className="w-12 h-12">
                <AvatarFallback className="bg-primary-light text-primary">
                  {expense.paidByName.charAt(0).toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <div className="flex-1">
                <div className="font-medium">{expense.paidByName}</div>
                <div className="text-sm text-muted-foreground">Paid ${expense.amount.toFixed(2)}</div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Split Details */}
        <Card className="shadow-card border-0">
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">Split details</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            {expense.allocations.map((allocation) => (
              <div key={allocation.participantId} className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <Avatar className="w-8 h-8">
                    <AvatarFallback className="text-xs bg-primary-light text-primary">
                      {allocation.participantName.charAt(0).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  <span className="font-medium">{allocation.participantName}</span>
                </div>
                <Badge variant="secondary" className="rounded-full">
                  ${allocation.amount.toFixed(2)}
                </Badge>
              </div>
            ))}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}