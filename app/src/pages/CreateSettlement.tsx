import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, Plus, X } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useToast } from '@/hooks/use-toast';

export function CreateSettlement() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const [settlementName, setSettlementName] = useState('');
  const [participants, setParticipants] = useState(['']);
  const [loading, setLoading] = useState(false);

  const addParticipant = () => {
    setParticipants([...participants, '']);
  };

  const removeParticipant = (index: number) => {
    if (participants.length > 1) {
      setParticipants(participants.filter((_, i) => i !== index));
    }
  };

  const updateParticipant = (index: number, value: string) => {
    const updated = [...participants];
    updated[index] = value;
    setParticipants(updated);
  };

  const handleCreate = async () => {
    if (!settlementName.trim()) {
      toast({
        title: "Error",
        description: "Please enter a settlement name",
        variant: "destructive",
      });
      return;
    }

    const validParticipants = participants.filter(p => p.trim());
    if (validParticipants.length === 0) {
      toast({
        title: "Error",
        description: "Please add at least one participant",
        variant: "destructive",
      });
      return;
    }

    setLoading(true);
    // Simulate API call
    setTimeout(() => {
      setLoading(false);
      toast({
        title: "Success",
        description: "Settlement created successfully!",
      });
      navigate('/dashboard');
    }, 1000);
  };

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <div className="bg-white border-b border-border p-4">
        <div className="flex items-center gap-3">
          <Button 
            variant="ghost" 
            size="icon"
            onClick={() => navigate('/dashboard')}
            className="shrink-0"
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
          
          <div className="flex-1">
            <h1 className="text-xl font-bold text-foreground">Create Settlement</h1>
            <p className="text-sm text-muted-foreground">Set up a new expense settlement</p>
          </div>
        </div>
      </div>

      <div className="p-4 space-y-6">
        <Card className="shadow-card border-0">
          <CardHeader>
            <CardTitle>Settlement Details</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="settlementName">Settlement Name</Label>
              <Input
                id="settlementName"
                type="text"
                placeholder="e.g., Weekend Trip, Dinner Split"
                value={settlementName}
                onChange={(e) => setSettlementName(e.target.value)}
                className="h-12 rounded-xl"
              />
            </div>
          </CardContent>
        </Card>

        <Card className="shadow-card border-0">
          <CardHeader>
            <CardTitle>Participants</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {participants.map((participant, index) => (
              <div key={index} className="flex gap-2">
                <Input
                  type="text"
                  placeholder={`Participant ${index + 1} name`}
                  value={participant}
                  onChange={(e) => updateParticipant(index, e.target.value)}
                  className="h-12 rounded-xl flex-1"
                />
                {participants.length > 1 && (
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => removeParticipant(index)}
                    className="h-12 w-12 shrink-0"
                  >
                    <X className="w-4 h-4" />
                  </Button>
                )}
              </div>
            ))}
            
            <Button
              variant="secondary"
              onClick={addParticipant}
              className="w-full h-12"
            >
              <Plus className="mr-2 w-4 h-4" />
              Add Participant
            </Button>
          </CardContent>
        </Card>

        <Button 
          onClick={handleCreate}
          disabled={loading}
          className="w-full h-14"
        >
          {loading ? 'Creating...' : 'Create Settlement'}
        </Button>
      </div>
    </div>
  );
}